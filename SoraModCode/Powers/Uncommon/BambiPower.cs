using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Cards.Special;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class BambiPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    // Set to Counter so the number represents the remaining turns, not intensity!
    public override PowerStackType StackType => PowerStackType.Counter; 

    // The Turn-Start Hook
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == this.Owner.Side)
        {
            // 1. Create the Drive Converter token
            var converter = combatState.CreateCard<DriveConverterSoraMod>(this.Owner.Player);

            // 2. Add it to the hand safely using our verified command
            await CardPileCmd.Add(converter, PileType.Hand);

            // 3. Tick the turn timer down! (Will auto-remove Bambi when he hits 0)
            await PowerCmd.Decrement(this);
        }
        await base.BeforeSideTurnStart(choiceContext, side, combatState);
    }
}