using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Basic;

public class FireSoraMod() : SoraModCard(2, CardType.Attack, CardRarity.Basic, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag> { SoraModEnums.Magic };
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(4m, ValueProp.Move) };
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}