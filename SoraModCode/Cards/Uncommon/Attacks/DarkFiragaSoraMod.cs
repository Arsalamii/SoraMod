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
public class DarkFiragaSoraMod : SoraMagicCard
{
    public DarkFiragaSoraMod() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    // 1. SET BASE STATS: 14 Damage, 1 Dazed
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> 
    { 
        new DamageVar(14m, ValueProp.Move),
        new DynamicVar("Status", 1m) 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // 3. DEAL DAMAGE
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 4. ADD DAZED TO DISCARD PILE
        int statusAmount = (int)this.DynamicVars["Status"].BaseValue;
        
        for (int i = 0; i < statusAmount; i++)
        {
            var generatedCardResult = await CardPileCmd.AddGeneratedCardToCombat(new Dazed(), PileType.Discard, true);
            
            CardCmd.PreviewCardPileAdd(generatedCardResult, 2.2f);
        }
    }

    // 5. UPGRADE: +4 Damage (from 14 to 18)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}