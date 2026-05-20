using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Enums;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Powers.Forms;

public class AntiFormListenerPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.None; 
    
    // THIS works perfectly for Powers, keeping the surprise intact!
    protected override bool IsVisibleInternal => false; 

    // 1. THE PAUSE (Inherited from AbstractModel)
    public override bool ShouldDieLate(Creature creature)
    {
        // Only trigger if it's our owner dying
        if (creature == this.Owner)
        {
            return false; // Prevent Death!
        }
        return true; 
    }

    // 2. THE RESCUE (Inherited from AbstractModel)
    public override async Task AfterPreventingDeath(Creature creature)
    {
        var crown = creature.Player!.Relics.OfType<BronzeCrown>().FirstOrDefault();
        if (crown != null)
        {
            crown.HasUsedAntiForm = true; // Flips the memory switch!
        }
        
        // 1. Prevent fatal damage and heal to 30% HP
        decimal targetHp = (decimal)creature.MaxHp * 0.3m;
        await CreatureCmd.Heal(creature, targetHp);

        // 2. Apply the visible Anti-Form State (Double Damage / No Heal)
        await PowerCmd.Apply<AntiFormPower>(creature, 1m, creature, null);

        // (Make sure to add: using SoraMod.SoraModCode.Enums; at the top of your file!)

        // 3. Exhaust all Skill, Power, and Magic cards
        var playerState = creature.Player!.PlayerCombatState; 
        var cardsToExhaust = new List<CardModel>();

        // Our clean helper rule to catch Skills, Powers, and Magic!
        bool ShouldExhaust(CardModel c) => 
            c.Type == CardType.Skill || 
            c.Type == CardType.Power || 
            c.Tags.Contains(SoraModEnums.Magic);

        cardsToExhaust.AddRange(playerState.Hand.Cards.Where(ShouldExhaust));
        cardsToExhaust.AddRange(playerState.DrawPile.Cards.Where(ShouldExhaust));
        cardsToExhaust.AddRange(playerState.DiscardPile.Cards.Where(ShouldExhaust));

        foreach(var card in cardsToExhaust)
        {
            await CardPileCmd.Add(card, PileType.Exhaust, CardPilePosition.Top, this, true);
        }

        // 4. Find all Keyblades in the game and pick 3 random ones
        // (If 'AllCards' gives a red squiggle, type 'ModelDb.' and check autocomplete for the exact list name!)
        var allKeyblades = ModelDb.AllCards.Where(c => c.Tags.Contains(SoraModEnums.Keyblade)).ToList();
        var random = new System.Random(); 
        var generatedKeyblades = new List<CardModel>();

        for(int i = 0; i < 3; i++)
        {
            if (allKeyblades.Count > 0)
            {
                // Pick a random canonical Keyblade model from the database
                var randomKeybladeModel = allKeyblades[random.Next(allKeyblades.Count)];
        
                // Create a combat-ready copy of it
                // (If Rider throws an error about generics here, let me know!)
                var newCard = creature.CombatState.CreateCard(randomKeybladeModel, creature.Player);
                generatedKeyblades.Add(newCard);
            }
        }

        // 5. Add them to combat and the Hand
        await CardPileCmd.AddGeneratedCardsToCombat(generatedKeyblades, PileType.Hand, true);

        // 6. Force the cost to 0 and tell the UI to redraw!
        foreach(var keyblade in generatedKeyblades)
        {
            keyblade.EnergyCost.SetUntilPlayed(0);
        }

        // This is the magic line that updates the visual numbers on the screen
        playerState.RecalculateCardValues();

        // 7. Remove the invisible listener so Sora can actually die if he gets hit again!
        this.RemoveInternal();
    }
}