using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class BlizzagaSoraMod : SoraMagicCard
{
    public BlizzagaSoraMod() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    // Includes Damage, Weak, AND Vulnerable!
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(14m, ValueProp.Move),
        new PowerVar<WeakPower>(2m),
        new PowerVar<VulnerablePower>(1m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. DAMAGE
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. WEAK
        var weakAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<WeakPower>>().First().BaseValue;
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target, 
            weakAmount, 
            this.Owner.Creature, 
            this
        );

        // 3. VULNERABLE (Using the exact same LINQ trick for Vuln!)
        var vulnAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<VulnerablePower>>().First().BaseValue;
        await PowerCmd.Apply<VulnerablePower>(
            cardPlay.Target, 
            vulnAmount, 
            this.Owner.Creature, 
            this
        );
    }
}