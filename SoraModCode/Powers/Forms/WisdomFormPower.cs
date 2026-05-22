using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Synergy;

namespace SoraMod.SoraModCode.Powers.Forms;

public class WisdomFormPower : SoraModPower
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
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Magic) && 
            !(cardPlay.Card is WisdomFormSoraMod) && 
            !(cardPlay.Card is RevertSoraMod) && 
            !(cardPlay.Card is DriveFormsSoraMod))
        {
            await CardPileCmd.Draw(context, 1, this.Owner.Player);
        }
    
        await base.AfterCardPlayed(context, cardPlay);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Magic))
        {
            return 2m; 
        }
        return 0m;
    }
    
    public override decimal ModifyBlockAdditive(Creature? target, decimal amount, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource?.Owner?.Creature == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Magic))
        {
            return 2m; 
        }
        return 0m;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != this.Owner.Side) return;
        
        if (this.Owner.Player.PlayerCombatState.Stars > 0)
        {
            this.Owner.Player.PlayerCombatState.LoseStars(1m);
        }

        if (this.Owner.Player.PlayerCombatState.Stars <= 0)
        {
            var revertCard = this.Owner.Player.PlayerCombatState.AllCards.FirstOrDefault(c => c is RevertSoraMod);
            if (revertCard != null)
            {
                await CardCmd.Exhaust(choiceContext, revertCard);
            }
            this.RemoveInternal();
        }
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