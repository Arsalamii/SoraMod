using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class ThundagaSoraMod : SoraMagicCard
{
    public ThundagaSoraMod() : base(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
    }

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(10m, ValueProp.Move),
        new RepeatVar(4)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Attack", this.Owner.Character.CastAnimDelay);

        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitCount(this.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(this.CombatState)
            .Execute(choiceContext);
    }
}