using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Uncommon;

[Pool(typeof(SoraModCardPool))]
public class MagnetSplashSoraMod() : SoraModCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>
        {
            CardTag.Strike,
            SoraModEnums.Keyblade,
            SoraModEnums.Magic
        };
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(6m, ValueProp.Move) };
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