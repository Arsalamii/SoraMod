using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Common;

[Pool(typeof(SoraModCardPool))]
public class SlapshotSoraMod() : SoraKeybladeCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(3m, ValueProp.Move) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SlapshotSoraMod card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).WithHitCount(3).FromCard((CardModel) card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(2M);
}