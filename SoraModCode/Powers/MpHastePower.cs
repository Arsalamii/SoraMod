using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace SoraMod.SoraModCode.Powers;

public class MPHastePower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    
    // Accumulate means if you play two MP Hastes, you gain 2 Energy!
    public override PowerStackType StackType => PowerStackType.Counter; 

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        // 1. Safety check: Only trigger on our own turn!
        if (player != this.Owner.Player) 
        {
            return;
        }

        // 2. Look at the cards currently in our hand
        CardPile hand = player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        if (hand == null) 
        {
            return;
        }

        // 3. Check if ANY card in our hand has the Magic tag!
        bool hasMagicCard = hand.Cards.Any(c => c.Tags.Contains(SoraModEnums.Magic));

        // 4. If we found one, give the player their energy!
        if (hasMagicCard)
        {
            this.Flash();
            
            // Give energy equal to the amount of MP Haste stacks we have!
            // (If this.Amount throws an error, try this.Stacks.BaseValue)
            await PlayerCmd.GainEnergy(this.Amount, player);
        }

        await base.AfterPlayerTurnStart(context, player);
    }
}