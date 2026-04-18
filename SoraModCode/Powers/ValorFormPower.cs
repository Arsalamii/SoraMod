using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Powers;

public class ValorFormPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await UpdateKeybladeDamage();
        UpdateFirstAttackCost();
    }

    public void OnCardDrawn(CardModel card)
    {
        UpdateFirstAttackCost();
    }

    public void OnCardPlayed(CardModel card)
    {
        if (card.Type == CardType.Attack)
        {
            ResetAttackCosts();
        }
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

    private async Task UpdateKeybladeDamage()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards)
        {
            if (card.Tags.Contains(SoraModEnums.Keyblade))
            {
                card.DynamicVars.Damage.BaseValue += 2;
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
}