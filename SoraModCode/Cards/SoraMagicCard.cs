using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Saves.Runs;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards;

public abstract class SoraMagicCard(int cost, CardType type, CardRarity rarity, TargetType target) : SoraModCard(cost, type, rarity, target)
{
    // Gives every single card a unique, randomized serial number!
    public string MagicSerialNumber { get; set; } = System.Guid.NewGuid().ToString();
    
    private int _experience;

    [SavedProperty]
    public int Experience
    {
        get => this._experience;
        set
        {
            this.AssertMutable();
            this._experience = value;
        }
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("Exp", (decimal)this.Experience);
    }

    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag>() { SoraModEnums.Magic };
}