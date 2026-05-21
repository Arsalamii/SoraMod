using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SoraMod.SoraModCode.Cards.Common.Attacks;

public class BlizzardSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    // Single Target!
    public BlizzardSoraMod() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(60m, ValueProp.Move),
        new PowerVar<WeakPower>(1m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // We grab the Monster first, then its physical Creature body!
        var targetCreature = cardPlay.Target.Monster?.Creature;
        int hpBefore = targetCreature?.CurrentHp ?? 0;

        // 1. DEAL DAMAGE
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. APPLY WEAK 
        var weakAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<WeakPower>>().First().BaseValue;
    
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target, 
            weakAmount, 
            this.Owner.Creature, 
            this
        );

        // 3. FATAL CHECK (Using targetCreature instead of monster)
        if (targetCreature != null && (targetCreature.IsDead || (hpBefore > 0 && targetCreature.CurrentHp <= 0)))
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
        
            // Reverted to your MagicSerialNumber fix!
            CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
                c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
            );

            if (trueMasterCard is SoraMagicCard magicMasterCard)
            {
                if (magicMasterCard.Experience < EvolutionRequirement)
                {
                    magicMasterCard.Experience += 1; 
                
                    if (magicMasterCard.Experience > EvolutionRequirement)
                    {
                        magicMasterCard.Experience = EvolutionRequirement;
                    }

                    if (magicMasterCard.Experience >= EvolutionRequirement)
                    {
                        await this.EvolveIntoBlizzara(magicMasterCard);
                    }
                }
            }
        }
    }

    public async Task EvolveIntoBlizzara(SoraMagicCard masterDeckCard)
    {
        if (masterDeckCard != null)
        {
            var newBlizzara = this.CardScope.CreateCard<BlizzaraSoraMod>(this.Owner);

            if (this.IsUpgraded)
            {
                newBlizzara.UpgradeInternal();
                newBlizzara.FinalizeUpgradeInternal();
            }

            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newBlizzara); 
            }
        }
    }
    
    public override int MaxUpgradeLevel => 0;
}