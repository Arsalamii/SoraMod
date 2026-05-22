using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class ShadowSwarmSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. SET BASE STATS: 3 Draw, 2 Status Cards
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DynamicVar("Draw", 3m),
            new DynamicVar("Status", 2m)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ShadowSwarmSoraMod card = this;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 3. DRAW CARDS
        int drawAmount = (int)card.DynamicVars["Draw"].BaseValue;
        await CardPileCmd.Draw(choiceContext, drawAmount, card.Owner.Creature.Player);

        // 4. ADD SLIMED TO DISCARD PILE
        int statusAmount = (int)card.DynamicVars["Status"].BaseValue;
        
        for (int i = 0; i < statusAmount; i++)
        {
            var generatedCardResult = await CardPileCmd.AddGeneratedCardToCombat(new Slimed(), PileType.Discard, true);
            CardCmd.PreviewCardPileAdd(generatedCardResult, 2.2f);
        }
    }

    // 5. UPGRADE: +1 Draw
    protected override void OnUpgrade() 
    {
        this.DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}