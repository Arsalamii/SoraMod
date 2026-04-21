using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Uncommon;
  
[Pool(typeof(SoraModCardPool))]
public class AeroSoraMod : SoraModCard
{

    public AeroSoraMod() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }
    
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>
        {
            CardTag.Defend,
            SoraModEnums.Magic
        };
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new BlockVar(6m, ValueProp.Move) };
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(4m);
    }
}