using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class ReversalPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 
    protected override bool IsVisibleInternal => false; 

    // HOOK 1: THE CARD DRAW
    // This happens as the turn begins, perfectly setting up Sora's hand.
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == this.Owner.Side)
        {
            // Draw the extra cards using your verified Dodge Roll draw command syntax
            await CardPileCmd.Draw(choiceContext, this.Amount, this.Owner.Player);
        }
        await base.BeforeSideTurnStart(choiceContext, side, combatState);
    }

    // HOOK 2: THE ENERGY GAIN (Stolen directly from EnergyNextTurnPower!)
    // This happens AFTER the engine resets Sora's base energy, so it won't get wiped out.
    public override async Task AfterEnergyReset(Player player)
    {
        // Make sure it's Sora getting the energy reset
        if (player == this.Owner.Player)
        {
            // Gain the extra Energy using the native command (cast to decimal!)
            await PlayerCmd.GainEnergy((decimal)this.Amount, player);

            // Remove this power completely since it has fulfilled both delayed triggers
            await PowerCmd.Remove(this);
        }
    }
}