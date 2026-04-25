using MegaCrit.Sts2.Core.Entities.Cards;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards;

public abstract class SoraKeybladeCard(int cost, CardType type, CardRarity rarity, TargetType target)  : SoraModCard(cost, type, rarity, target)
{
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag>() { SoraModEnums.Keyblade };
}