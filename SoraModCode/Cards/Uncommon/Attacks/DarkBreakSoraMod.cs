using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class DarkBreakSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS: 14 Damage, 1 Regret
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(14m, ValueProp.Move),
            new DynamicVar("Status", 1m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DarkBreakSoraMod card = this;
        
        if (cardPlay.Target == null) return;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 3. DEAL DAMAGE
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 4. ADD REGRET TO DISCARD PILE
        int statusAmount = (int)card.DynamicVars["Status"].BaseValue;
        
        for (int i = 0; i < statusAmount; i++)
        {
            // Note: If 'new Regret()' throws an error, try looking for it in a sub-namespace 
            // like MegaCrit.Sts2.Core.Models.Cards.Curses
            var generatedCardResult = await CardPileCmd.AddGeneratedCardToCombat(new Regret(), PileType.Discard, true);
            
            // Pop the Regret visually on screen so the player knows what happened
            CardCmd.PreviewCardPileAdd(generatedCardResult, 2.2f);
        }
    }

    // 5. UPGRADE: +4 Damage (from 14 to 18)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(4m);
        // We leave the Status variable alone so it always stays at exactly 1!
    }
}