using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Common;

public class MagnetSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public MagnetSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    // 1. Declare the native Vulnerable variable!
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<VulnerablePower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        var masterDeck = PileType.Deck.GetPile(this.Owner);
        CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => c.Id == this.Id);

        foreach (var monster in this.CombatState.Enemies)
        {
            if (monster != null && !monster.IsDead)
            {
                // 1. Apply Vulnerable
                await PowerCmd.Apply<VulnerablePower>(
                    monster,
                    this.DynamicVars.Vulnerable.BaseValue,
                    this.Owner.Creature,
                    this
                );

                // 2. The Callback Logic
                if (trueMasterCard is SoraMagicCard magicMasterCard)
                {
                    // We pack the EXP and Evolution logic into a variable
                    Func<Task> onDeathAction = async () =>
                    {
                        // Safety check: Only gain EXP/Evolve if it hasn't already evolved!
                        if (magicMasterCard.Experience < EvolutionRequirement)
                        {
                            magicMasterCard.Experience += 1;

                            if (magicMasterCard.Experience >= EvolutionRequirement)
                            {
                                await this.EvolveIntoMagnera(magicMasterCard);
                            }
                        }
                    };

                    // Look for an existing tracker on this specific enemy
                    var existingTracker = monster.GetPower<AssistTrackerPower>();

                    if (existingTracker != null)
                    {
                        existingTracker.AddCallback(onDeathAction);
                    }
                    else
                    {
                        // 1. Catch the newly created tracker in a variable
                        var newTracker = await PowerCmd.Apply<AssistTrackerPower>(
                            monster,
                            1m,
                            this.Owner.Creature,
                            this
                        );

                        // 2. Hand it the suitcase!
                        if (newTracker != null)
                        {
                            newTracker.AddCallback(onDeathAction);
                        }
                    }
                }
            }
        }
    }

// ... (Keep your standard EvolveIntoMagnera manual swap logic here!) ...
    public async Task EvolveIntoMagnera(SoraMagicCard masterDeckCard)
    {
        // Because the Tracker Power will trigger the evolution, this method needs to be public!
        var newMagnera = this.CardScope.CreateCard<MagneraSoraMod>(this.Owner);
        if (this.IsUpgraded)
        {
            newMagnera.UpgradeInternal();
            newMagnera.FinalizeUpgradeInternal();
        }

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newMagnera); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<MagneraSoraMod>(this);
        }
    }
}