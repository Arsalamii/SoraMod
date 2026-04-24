using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace SoraMod.SoraModCode.Powers;

public class ComboMasterPower : SoraModPower
{
    // Our hidden tally for the current turn
    private int _keybladeCounter = 0;

    public override PowerType Type => PowerType.Buff;
    
    // Accumulate means playing multiple Combo Masters stacks the energy refund!
    public override PowerStackType StackType => PowerStackType.Counter; 

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 1. Did Sora just play a Keyblade?
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade) && cardPlay.Card.Owner.Creature == this.Owner)
        {
            // 2. Increase the counter
            this._keybladeCounter++;

            // 3. Did we hit the 3rd swing?
            if (this._keybladeCounter >= 3)
            {
                this.Flash(); 
                
                // 4. Refund the Energy! 
                // We cast this.Amount to an (int) just in case the engine is strict about whole numbers here.
                await PlayerCmd.GainEnergy((int)this.Amount, this.Owner.Player);

                // 5. Reset the counter!
                this._keybladeCounter = 0; 
            }
        }

        await base.AfterCardPlayed(context, cardPlay);
    }

    // Clear the counter at the start of a new turn
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (player == this.Owner.Player)
        {
            this._keybladeCounter = 0;
        }
        
        await base.AfterPlayerTurnStart(context, player);
    }
}