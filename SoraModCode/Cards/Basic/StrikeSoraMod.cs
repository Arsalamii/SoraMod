using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Basic;

public class StrikeSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(20m, ValueProp.Move) };
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}