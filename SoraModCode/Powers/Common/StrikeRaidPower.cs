using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Cards.Common;
using SoraMod.SoraModCode.Cards.Common.Attacks;

namespace SoraMod.SoraModCode.Powers.Common;

public class StrikeRaidPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    // We make it invisible so the player doesn't need to see a buff icon
    // for an effect that happens automatically!
    protected override bool IsVisibleInternal => false; 

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != this.Owner) return;

        // We will build a list of all the cards we need to generate
        var generatedCards = new List<CardModel>();

        // Run this loop once for every stack of the power!
        for (int i = 0; i < this.Amount; i++)
        {
            var tempCard = player.Creature.CombatState.CreateCard<StrikeRaidSoraMod>(player);
            tempCard.AddKeyword(CardKeyword.Ethereal);
            tempCard.AddKeyword(CardKeyword.Exhaust);
        
            generatedCards.Add(tempCard);
        }

        // Add ALL of them to the player's Hand at once
        await CardPileCmd.AddGeneratedCardsToCombat(generatedCards, PileType.Hand, true);
    
        // Force the UI to redraw them
        player.PlayerCombatState.RecalculateCardValues();

        // Remove the power so it resets completely
        this.RemoveInternal();
    }
}