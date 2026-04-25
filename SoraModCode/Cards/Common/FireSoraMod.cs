using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;

namespace SoraMod.SoraModCode.Cards.Common;

public class FireSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public FireSoraMod() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(50m, ValueProp.Move) 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int totalKillsThisPlay = 0;

        foreach (var monster in this.CombatState.Enemies.ToList())
        {
            if (monster != null && !monster.IsDead)
            {
                int hpBefore = monster.CurrentHp; 

                await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(monster)
                    .Execute(choiceContext);

                if (monster.IsDead || (hpBefore > 0 && monster.CurrentHp <= 0))
                {
                    totalKillsThisPlay++;
                }
            }
        }

        if (totalKillsThisPlay > 0)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            
            // 1. THE SERIAL NUMBER FIX: Re-added so multiple Fires track perfectly!
            CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
                c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
            );

            if (trueMasterCard is SoraMagicCard magicMasterCard)
            {
                // 2. THE EXP CAP: Prevents Fire from ever going to 5/3!
                if (magicMasterCard.Experience < EvolutionRequirement)
                {
                    magicMasterCard.Experience += totalKillsThisPlay;
                    
                    // Clamp it so it doesn't overshoot
                    if (magicMasterCard.Experience > EvolutionRequirement)
                    {
                        magicMasterCard.Experience = EvolutionRequirement;
                    }

                    if (magicMasterCard.Experience >= EvolutionRequirement)
                    {
                        await this.EvolveIntoFira(magicMasterCard);
                    }
                }
            }
        }
    }

    public async Task EvolveIntoFira(SoraMagicCard masterDeckCard)
    {
        if (masterDeckCard != null)
        {
            // 1. Create the new Fira
            var newFira = this.CardScope.CreateCard<FiraSoraMod>(this.Owner);

            // 2. Transfer the Upgrades
            if (this.IsUpgraded)
            {
                newFira.UpgradeInternal();
                newFira.FinalizeUpgradeInternal();
            }

            // 3. The True "Pokémon" Swap in the Master Deck
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                
                // We use AddInternal because it perfectly inserts the card into the Run's Master Deck.
                // It securely completes the evolution for the next room!
                masterDeck.AddInternal(newFira); 
            }
        }
    }
}