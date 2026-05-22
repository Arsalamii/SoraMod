using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Enums;
using SoraMod.SoraModCode.Synergy;

namespace SoraMod.SoraModCode.Powers.Forms;

public class ValorFormPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateFirstKeybladeCost();
        TriggerAllFormSynergies(); // INTERFACE TRIGGER
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        UpdateFirstKeybladeCost();
        
        // INTERFACE TRIGGER
        if (card is IDriveFormSynergy synergyCard)
        {
            synergyCard.ApplyDriveSynergy();
        }
        
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            ResetKeybladeCosts();
        }
        return base.AfterCardPlayed(context, cardPlay);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        RemoveAllFormSynergies(); // INTERFACE TRIGGER
        
        if (oldOwner.Player?.PlayerCombatState != null)
        {
            CardPile hand = oldOwner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
            if (hand != null)
            {
                foreach (CardModel card in hand.Cards)
                {
                    if (card.Tags.Contains(SoraModEnums.Keyblade))
                    {
                        card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
                    }
                }
            }
        }
    }

    // --- YOUR EXISTING KEYBLADE LOGIC ---
    private void UpdateFirstKeybladeCost()
    {
        if (HasPlayedKeybladeThisTurn) return;

        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards)
        {
            if (card.Tags.Contains(SoraModEnums.Keyblade))
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(0);
            }
        }
    }

    private void ResetKeybladeCosts()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards)
        {
            if (card.Tags.Contains(SoraModEnums.Keyblade))
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
            }
        }
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Keyblade))
        {
            int previousKeyblades = CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Count(e =>
                e.HappenedThisTurn(this.CombatState) && 
                e.CardPlay.Card.Owner.Creature == this.Owner && 
                e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade) &&
                e.CardPlay.Card != cardSource); 

            return previousKeyblades * 2m; 
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

    private int KeybladesPlayedThisTurn => CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Count(e =>
        e.HappenedThisTurn(this.CombatState) && 
        e.CardPlay.Card.Owner.Creature == this.Owner && 
        e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade));
    
    private bool HasPlayedKeybladeThisTurn => KeybladesPlayedThisTurn > 0;

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