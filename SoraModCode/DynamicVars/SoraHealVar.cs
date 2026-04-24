using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Powers;

// Make sure to add your using statement for WisdomFormPower!

namespace SoraMod.SoraModCode.DynamicVars; // Or your preferred namespace

public class SoraHealVar : HealVar
{
    public SoraHealVar(decimal healAmount) : base(healAmount)
    {
    }

    // This is the magic engine hook! It runs every frame the card is in your hand.
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);

        decimal bonusHeal = 0;

        // Does Sora have Wisdom Form active?
        if (card.Owner != null && card.Owner.HasPower<WisdomFormPower>())
        {
            bonusHeal += 3; // Or whatever bonus you want to give!
        }

        // By updating the PreviewValue, the :diff() parser instantly turns the text green!
        this.PreviewValue = this.EnchantedValue + bonusHeal;
    }
}