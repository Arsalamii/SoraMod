using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Powers;

[Pool(typeof(SoraModCardPool))]
public class SummonBambiSoraMod : SoraModCard
{
    public SummonBambiSoraMod() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // Apply 3 Stacks of Bambi (Representing our 3 Turns!)
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<BambiPower>(3m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Buff", this.Owner.Character.CastAnimDelay);

        var turns = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<BambiPower>>().First().BaseValue;
        
        await PowerCmd.Apply<BambiPower>(this.Owner.Creature, turns, this.Owner.Creature, this);
    }

    // THE UPGRADE (Gains Innate)
    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}