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
        UpdateFirstAttackCost();
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        UpdateFirstAttackCost();
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack)
        {
            ResetAttackCosts();
        }
        return base.AfterCardPlayed(context, cardPlay);
    }

    private void UpdateFirstAttackCost()
    {
        if (HasPlayedAttackThisTurn) return;

        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards)
        {
            if (card.Type == CardType.Attack)
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(Math.Max(0, card.EnergyCost.Canonical - 2));
            }
        }
    }

    private void ResetAttackCosts()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards)
        {
            if (card.Type == CardType.Attack)
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
            }
        }
    }

    // The engine automatically asks this method how much extra damage a card should do!
    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // If we are the one attacking, and the card being used has the Keyblade tag...
        if (dealer == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Keyblade))
        {
            return 2m; // ...tell the engine to add 2 damage to it!
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
            // The form is dropping! Find the Revert card...
            var revertCard = this.Owner.Player.PlayerCombatState.AllCards.FirstOrDefault(c => c is RevertSoraMod);
            
            if (revertCard != null)
            {
                // ...and officially burn it using the ChoiceContext!
                await CardCmd.Exhaust(choiceContext, revertCard);
            }

            this.RemoveInternal();
        }
    }

    private bool HasPlayedAttackThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Any(e =>
                e.HappenedThisTurn(this.CombatState) && e.CardPlay.Card.Owner.Creature == this.Owner && e.CardPlay.Card.Type == CardType.Attack);
        }
    }
    
    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.Player?.PlayerCombatState != null)
        {
            CardPile hand = oldOwner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
            if (hand != null)
            {
                // We only need to reset attack costs here now! 
                // Revert is safely handled by its own Exhaust keyword, or the TurnEnd hook above!
                foreach (CardModel card in hand.Cards)
                {
                    if (card.Type == CardType.Attack)
                    {
                        card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
                    }
                }
            }
        }
    }
}