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

namespace SoraMod.SoraModCode.Powers;

public class ValorFormPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateFirstKeybladeCost();
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        UpdateFirstKeybladeCost();
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Changed to look for the Keyblade tag!
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            ResetKeybladeCosts();
        }
        return base.AfterCardPlayed(context, cardPlay);
    }

    private void UpdateFirstKeybladeCost()
    {
        if (HasPlayedKeybladeThisTurn) return;

        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards)
        {
            // Changed to look for the Keyblade tag!
            if (card.Tags.Contains(SoraModEnums.Keyblade))
            {
                // Hardcoded to 0 cost!
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
            // Changed to look for the Keyblade tag!
            if (card.Tags.Contains(SoraModEnums.Keyblade))
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
            }
        }
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Keyblade))
        {
            // We recount the history right here, but add a strict rule to IGNORE the cardSource!
            int previousKeyblades = CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Count(e =>
                e.HappenedThisTurn(this.CombatState) && 
                e.CardPlay.Card.Owner.Creature == this.Owner && 
                e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade) &&
                e.CardPlay.Card != cardSource); // <--- THE MAGIC FIX!

            return previousKeyblades * 2m; 
        }
        
        return 0m;
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != this.Owner.Side)
        {
            return;
        }
        
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

    // NEW PROPERTY: Physically tallies up your combo for the turn
    private int KeybladesPlayedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Count(e =>
                e.HappenedThisTurn(this.CombatState) && 
                e.CardPlay.Card.Owner.Creature == this.Owner && 
                e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade));
        }
    }
    
    // NEW PROPERTY: A simple true/false check using our combo tally
    private bool HasPlayedKeybladeThisTurn => KeybladesPlayedThisTurn > 0;
    
    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.Player?.PlayerCombatState != null)
        {
            CardPile hand = oldOwner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
            if (hand != null)
            {
                foreach (CardModel card in hand.Cards)
                {
                    // Changed to look for the Keyblade tag!
                    if (card.Tags.Contains(SoraModEnums.Keyblade))
                    {
                        card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
                    }
                }
            }
        }
    }
}