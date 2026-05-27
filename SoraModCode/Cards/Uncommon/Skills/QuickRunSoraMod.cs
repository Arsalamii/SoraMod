using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class QuickRunSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public QuickRunSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. SET BASE STATS: 5 Block, 1 Quick Run Power
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(5m, ValueProp.Move),
        new PowerVar<QuickRunPower>(1m)
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Play the Defend animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        // Sora gains the Block
        await CommonActions.CardBlock(this, cardPlay);
        
        var buffAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<QuickRunPower>>().First().BaseValue;

        // Apply the QuickRunPower to discount the next Keyblade
        await PowerCmd.Apply<QuickRunPower>(
            this.Owner.Creature, 
            buffAmount, 
            this.Owner.Creature, 
            this
        );
    }

    // 4. THE UPGRADE
    protected override void OnUpgrade()
    {
        // Upgrades Block by 3 (from 5 to 8)
        this.DynamicVars.Block.UpgradeValueBy(3m);
        
        // (If you ever wanted to upgrade the power stacks instead, you would do this:)
        // var buffVar = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<QuickRunPower>>().First();
        // buffVar.UpgradeValueBy(1m);
    }
}