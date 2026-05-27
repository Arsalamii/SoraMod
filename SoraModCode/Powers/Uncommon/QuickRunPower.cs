using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class QuickRunPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 
    protected override bool IsVisibleInternal => false; 

    // 1. THE COST MODIFIER HOOK (Stolen from Corruption!)
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        // If it's not our card, or it DOESN'T have the Keyblade tag, ignore it
        if (card.Owner.Creature != this.Owner || !card.Tags.Contains(SoraModEnums.Keyblade))
        {
            modifiedCost = originalCost;
            return false; 
        }

        // It IS a Keyblade! Reduce cost by 1 (but don't go below 0)
        modifiedCost = Math.Max(0m, originalCost - 1m);
        return true; // Tell the engine to apply the discount!
    }

    // 2. THE CONSUMPTION HOOK
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // If we successfully played a Keyblade, use up one stack
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            // Submit the server command to reduce stacks
            await PowerCmd.ModifyAmount(this, -1m, this.Owner, cardPlay.Card);
            
            // If we are out of charges, remove the power completely
            if (this.Amount <= 0)
            {
                await PowerCmd.Remove(this);
            }
        }
        await base.AfterCardPlayed(choiceContext, cardPlay);
    }

    // 3. THE EXPIRATION HOOK
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // "This turn" means we wipe the buff when Sora ends his turn
        if (side == this.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
        await base.BeforeTurnEnd(choiceContext, side);
    }
}