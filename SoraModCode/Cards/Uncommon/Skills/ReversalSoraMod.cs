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
public class ReversalSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR (Cost 1, Target Self)
    public ReversalSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. SET BASE STATS: 6 Block, 1 Power Stack (which correlates to 1 Energy / 1 Card)
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(6m, ValueProp.Move),
        new PowerVar<ReversalPower>(1m)
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Play the Defend animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        // Sora gains his standard Block via the native BaseLib helper
        await CommonActions.CardBlock(this, cardPlay);

        // Safely extract the power stack value using your working LINQ query structure
        var buffAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<ReversalPower>>().First().BaseValue;

        // Apply the ReversalPower to set up the next turn's rewards!
        await PowerCmd.Apply<ReversalPower>(
            this.Owner.Creature, 
            buffAmount, 
            this.Owner.Creature, 
            this
        );
    }

    // 4. THE UPGRADE: Upgrade Block by 3 (from 6 to 9)
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3m);
    }
}