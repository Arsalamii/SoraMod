using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Synergy;

namespace SoraMod.SoraModCode.Powers.Forms;

public class AntiFormPower : SoraModPower, IHealAmountModifier
{
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Single;

    // --- NEW INTERFACE TRIGGERS ---
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        TriggerAllFormSynergies();
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is IDriveFormSynergy synergyCard)
        {
            synergyCard.ApplyDriveSynergy();
        }
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        RemoveAllFormSynergies();
    }

    // --- YOUR EXISTING LOGIC ---
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == this.Owner)
        {
            return 2m; // Deal Double Damage!
        }
        return 1m; 
    }

    public Decimal ModifyHealMultiplicative(Creature creature, Decimal amount)
    {
        if (creature == this.Owner)
        {
            return 0m; // Multiply any incoming heal by 0
        }
        return 1m; 
    }

    // --- NEW INTERFACE HELPERS ---
    private void TriggerAllFormSynergies()
    {
        var hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (var card in hand.Cards.OfType<IDriveFormSynergy>())
        {
            card.ApplyDriveSynergy();
        }
    }

    private void RemoveAllFormSynergies()
    {
        var hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (var card in hand.Cards.OfType<IDriveFormSynergy>())
        {
            card.RemoveDriveSynergy();
        }
    }
}