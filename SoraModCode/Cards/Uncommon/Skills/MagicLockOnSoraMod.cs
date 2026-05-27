using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class MagicLockOnSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public MagicLockOnSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    // 2. SET BASE STATS
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        // 1 Vulnerable (This is the one we will upgrade!)
        new PowerVar<VulnerablePower>(1m),
        // 1 Magic Lock-On Stack
        new PowerVar<MagicLockOnPower>(1m)
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        System.ArgumentNullException.ThrowIfNull((object)cardPlay.Target, "cardPlay.Target");

        // 1. Apply Vulnerable to the target enemy
        var vulnAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<VulnerablePower>>().First().BaseValue;
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, vulnAmount, this.Owner.Creature, this);

        // 2. Apply the Lock-On buff to Sora
        var buffAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<MagicLockOnPower>>().First().BaseValue;
        await PowerCmd.Apply<MagicLockOnPower>(this.Owner.Creature, buffAmount, this.Owner.Creature, this);
    }

    // 4. THE UPGRADE (Apply 2 Vulnerable instead of 1)
    protected override void OnUpgrade()
    {
        var vulnVar = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<VulnerablePower>>().First();
        vulnVar.UpgradeValueBy(1m);
    }
}