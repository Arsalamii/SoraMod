using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SoraMod.SoraModCode.Enums;

  
public static class SoraModEnums
{
    [CustomEnum] public static CardTag Keyblade;
    [CustomEnum] public static CardTag Magic;

    [CustomEnum, KeywordProperties((AutoKeywordPosition.Before))]
    public static CardKeyword Form;
}