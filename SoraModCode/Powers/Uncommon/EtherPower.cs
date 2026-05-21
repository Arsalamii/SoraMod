using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Cards;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class EtherPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => false; 

    // 1. UPDATE COSTS WHEN APPLIED OR DRAWN
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateMagicCosts();
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        UpdateMagicCosts();
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    // 2. CONSUME A STACK WHEN A MAGIC CARD IS PLAYED
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is SoraMagicCard)
        {
            // Safely reduce the amount by 1!
            this.SetAmount(this.Amount - 1);
            
            if (this.Amount <= 0)
            {
                ResetMagicCosts();
                this.RemoveInternal();
            }
        }
        
        await base.AfterCardPlayed(context, cardPlay);
    }

    // 3. EXPIRE AT END OF TURN
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == this.Owner.Side)
        {
            ResetMagicCosts();
            this.RemoveInternal();
        }
        await base.AfterTurnEnd(choiceContext, side);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        ResetMagicCosts();
    }

    // --- HELPER METHODS FROM VALOR FORM ---
    private void UpdateMagicCosts()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards.Where(c => c is SoraMagicCard))
        {
            card.EnergyCost.SetThisTurnOrUntilPlayed(0);
        }
    }

    private void ResetMagicCosts()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards.Where(c => c is SoraMagicCard))
        {
            card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
        }
    }
}