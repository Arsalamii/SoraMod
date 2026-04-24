using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SoraMod.SoraModCode.Powers;

public class KeybladeMasteryPower : SoraModPower
{
    // Our hidden tally for the current turn
    private int _keybladeCounter = 0;

    public override PowerType Type => PowerType.Buff;
    
    // Using Accumulate means playing multiple Keyblade Masteries stacks the reward!
    // (e.g., 2 stacks = 2 Dex every 3 cards)
    public override PowerStackType StackType => PowerStackType.Counter; 

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 1. Did Sora just play a Keyblade?
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade) && cardPlay.Card.Owner.Creature == this.Owner)
        {
            // 2. Increase our hidden counter!
            this._keybladeCounter++;

            // 3. Did we hit the magic number 3?
            if (this._keybladeCounter >= 3)
            {
                this.Flash(); // Give the power a nice visual pop on the UI!
                
                // 4. Grant the Dexterity! 
                // (Note: Make sure DexterityPower turns blue/teal. If it's red, Alt+Enter to import the STS2 core powers!)
                await PowerCmd.Apply<DexterityPower>(
                    this.Owner,
                    this.Amount, // We use this.Amount so stacked powers grant more Dex!
                    this.Owner,
                    null
                );

                // 5. Reset the counter so they can trigger it again in the same turn!
                this._keybladeCounter = 0; 
            }
        }

        await base.AfterCardPlayed(context, cardPlay);
    }

    // Reset the counter at the start of a new turn to prevent carrying over half-finished combos
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (player == this.Owner.Player)
        {
            this._keybladeCounter = 0;
        }
        
        await base.AfterPlayerTurnStart(context, player);
    }
}