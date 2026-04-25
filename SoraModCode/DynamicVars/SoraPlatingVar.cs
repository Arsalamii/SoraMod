using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Powers;

// Make sure you have your using statement for WisdomFormPower here!

namespace SoraMod.SoraModCode.DynamicVars;

public class SoraPlatingVar : DynamicVar
{
    public SoraPlatingVar(decimal amount) : base("Plating", amount)
    {
    }

    // The UI Hook to make the text turn green!
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);

        decimal bonus = 0;

        // Does Sora have Wisdom Form active?
        if (card.Owner != null && card.Owner.HasPower<WisdomFormPower>())
        {
            bonus += 2; // The Wisdom Form bonus! (Adjust this number to whatever you like)
        }

        this.PreviewValue = this.EnchantedValue + bonus;
    }
}