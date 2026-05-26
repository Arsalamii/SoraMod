using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class ReflegaSoraMod : SoraMagicCard
{
    public ReflegaSoraMod() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(24m, ValueProp.Move) // Upgraded to 24 Block
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        // 1. GAIN BLOCK
        await CommonActions.CardBlock(this, cardPlay);

        // 2. APPLY REFLECT POWER
        await PowerCmd.Apply<SoraReflectPower>(this.Owner.Creature, 1m, this.Owner.Creature, this);
    }
}