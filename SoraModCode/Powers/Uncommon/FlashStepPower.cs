using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class FlashStepPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => false; 

    // 1. UPDATE COSTS WHEN APPLIED OR DRAWN
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateKeybladeCosts();
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        UpdateKeybladeCosts();
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    // 2. CONSUME A STACK WHEN A KEYBLADE IS PLAYED
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            // Safely reduce the stack!
            this.SetAmount(this.Amount - 1);
            
            if (this.Amount <= 0)
            {
                ResetKeybladeCosts();
                this.RemoveInternal();
            }
        }
        
        await base.AfterCardPlayed(context, cardPlay);
    }

    // 3. EXPIRE AT THE END OF THE TURN
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == this.Owner.Side)
        {
            ResetKeybladeCosts();
            this.RemoveInternal();
        }
        await base.AfterTurnEnd(choiceContext, side);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        ResetKeybladeCosts();
    }

    // --- VISUAL HELPERS ---
    private void UpdateKeybladeCosts()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards.Where(c => c.Tags.Contains(SoraModEnums.Keyblade)))
        {
            card.EnergyCost.SetThisTurnOrUntilPlayed(0);
        }
    }

    private void ResetKeybladeCosts()
    {
        CardPile hand = this.Owner.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) return;

        foreach (CardModel card in hand.Cards.Where(c => c.Tags.Contains(SoraModEnums.Keyblade)))
        {
            card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
        }
    }
}