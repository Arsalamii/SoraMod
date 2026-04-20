using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Powers;

public class ValorFormPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateKeybladeDamage();
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

    private void UpdateKeybladeDamage()
    {
        if (this.Owner.Player?.PlayerCombatState != null)
        {
            foreach (CardModel card in this.Owner.Player.PlayerCombatState.AllCards)
            {
                if (card.Tags.Contains(SoraModEnums.Keyblade))
                {
                    card.DynamicVars.Damage.BaseValue += 2;
                }
            }
        }
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (this.Owner.Player.PlayerCombatState.Stars > 0)
        {
            this.Owner.Player.PlayerCombatState.LoseStars(1m);
        }

        if (this.Owner.Player.PlayerCombatState.Stars <= 0)
        {
            this.RemoveInternal();
        }
        return Task.CompletedTask;
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
            // Loop through EVERY card the player owns in combat (Hand, Draw, Discard, Exhaust!)
            foreach (CardModel card in oldOwner.Player.PlayerCombatState.AllCards.ToList())
            {
                // 1. Strip the damage buff
                if (card.Tags.Contains(SoraModEnums.Keyblade))
                {
                    card.DynamicVars.Damage.BaseValue -= 2;
                }

                // 2. Reset lingering attack costs
                if (card.Type == CardType.Attack)
                {
                    card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
                }

                // 3. Nuke Revert from orbit!
                if (card is RevertSoraMod)
                {
                    await CardPileCmd.RemoveFromCombat(card, false);
                }
            }
        }
    }
}