using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class MagicLockOnPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => false;

    // 1. THE DUPLICATION MODIFIER
    // The engine natively asks this hook how many times the card should trigger.
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        // If it's our card AND it has the Magic tag, tell the engine to play it one extra time!
        if (card.Owner.Creature == this.Owner && card.Tags.Contains(SoraModEnums.Magic))
        {
            return playCount + 1;
        }
        
        return playCount;
    }

    // 2. THE CONSUMPTION HOOK
    // This fires immediately after the engine accepts our modified play count.
    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        // Decrement automatically subtracts 1 stack and removes the power if it hits 0!
        await PowerCmd.Decrement(this);
    }

    // 3. THE EXPIRATION HOOK
    // Burst uses AfterTurnEnd instead of BeforeTurnEnd, so we match it to be safe.
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == this.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
        await base.AfterTurnEnd(choiceContext, side);
    }
}