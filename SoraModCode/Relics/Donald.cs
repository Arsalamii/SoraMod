using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Relics;

public class Donald() : SoraModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    // Hook directly into the end of the player's turn
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.BeforeTurnEnd(choiceContext, side);

        // We only want Donald to act on the Player's turn!
        if (side == CombatSide.Player)
        {
            var discardPile = PileType.Discard.GetPile(this.Owner);

            // 1. Filter the discard pile for ONLY Magic cards
            var magicCards = discardPile?.Cards.Where(c => c.Tags.Contains(SoraModEnums.Magic)).ToList();

            // 2. Did Donald find any spells?
            if (magicCards != null && magicCards.Count > 0)
            {
                this.Flash();

                // 3. THE SNEAKY BACKDOOR FIX!
                // We just steal the CombatState reference directly from the card!
                var combatState = magicCards[0].CombatState;

                // 4. Pick a random Magic card using the card's RNG
                var rng = this.Owner.PlayerRng.Transformations; 
                var randomCardIndex = rng.NextInt(0, magicCards.Count);
                var cardToPlay = magicCards[randomCardIndex];

                // 5. Find a random living enemy
                var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();

// (Added the ? to Creature)
                Creature? randomEnemy = null;

                if (enemies.Count > 0)
                {
                    // (Used your brilliant rng.NextInt discovery!)
                    randomEnemy = enemies[rng.NextInt(0, enemies.Count)];
                }

                // 6. Tell the engine to auto-play the card!
                await CardCmd.AutoPlay(choiceContext, cardToPlay, randomEnemy);
            }
        }
    }
}