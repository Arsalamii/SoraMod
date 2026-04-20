using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Rare;

[Pool(typeof(SoraModCardPool))]
public class ReactionCommandSoraMod() : SoraModCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 1. REGISTER THE BLOCK
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new BlockVar(15m, ValueProp.Move) };
    }

    // 2. THE PLAY SEQUENCE
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ReactionCommandSoraMod card = this;

        // Step 1: Gain the 15 Block
        await CreatureCmd.GainBlock(card.Owner.Creature, card.DynamicVars.Block, cardPlay);

        // Step 2: Apply the Reaction Command buff to listen for the attack!
        await PowerCmd.Apply<ReactionCommandPower>(
            card.Owner.Creature, 
            1m, // Amount doesn't really matter here since StackType is None
            card.Owner.Creature, 
            card
        );
    }

    // 3. THE UPGRADE (Cost reduction)
    protected override void OnUpgrade()
    {
        // In StS2, to upgrade energy cost you manipulate the EnergyCost variable.
        // Use autocomplete on `this.EnergyCost.` if this exact method name throws a red line!
        // It might be something like .UpgradeBaseValueBy(-1m) or .UpgradeCost(1);
        this.EnergyCost.UpgradeBy(-1); 
    }
}