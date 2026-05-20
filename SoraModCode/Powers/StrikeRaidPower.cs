using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Cards.Common;

namespace SoraMod.SoraModCode.Powers;

public class StrikeRaidPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    
    // We make it invisible so the player doesn't need to see a buff icon
    // for an effect that happens automatically!
    protected override bool IsVisibleInternal => false; 

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // Make sure we only trigger for the player who has the power
        if (player.Creature != this.Owner) return;

        // 1. Create the temporary Strike Raid using the Creature's combat state
        var tempCard = player.Creature.CombatState.CreateCard<StrikeRaidSoraMod>(player);
        
        // 2. Make it Exhaust! 
        tempCard.ExhaustOnNextPlay = true; 
        
        // 3. Add it to the player's Hand
        await CardPileCmd.AddGeneratedCardsToCombat(new List<CardModel> { tempCard }, PileType.Hand, true);
        
        // 4. Remove this power so it only happens once
        this.RemoveInternal();
    }
}