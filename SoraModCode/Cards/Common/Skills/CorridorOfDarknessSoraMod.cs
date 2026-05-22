using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps; 
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class CorridorOfDarknessSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. SET BASE STATS: 3 HP Loss, 2 Draw
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new HpLossVar(3m), 
            new DynamicVar("Draw", 2m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CorridorOfDarknessSoraMod card = this;

        // 2. PLAY ANIMATION 
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 3. LOSE HP
        // We use CreatureCmd.Damage but tell it to bypass Block and Power multipliers
        await CreatureCmd.Damage(
            choiceContext, 
            card.Owner.Creature, 
            card.DynamicVars.HpLoss.BaseValue, 
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, 
            card
        );

        // 4. DRAW CARDS
        // Using the exact Creature.Player routing we locked in!
        int drawAmount = (int)card.DynamicVars["Draw"].BaseValue;
        await CardPileCmd.Draw(choiceContext, drawAmount, card.Owner.Creature.Player);
    }

    // 5. UPGRADE: -1 HP Loss (from 3 to 2)
    protected override void OnUpgrade() 
    {
        // Upgrading with a negative number reduces the health cost!
        this.DynamicVars.HpLoss.UpgradeValueBy(-1m);
    }
}