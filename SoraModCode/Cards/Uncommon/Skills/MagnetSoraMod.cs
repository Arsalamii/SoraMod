using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Forms;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class MagnetSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public MagnetSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<VulnerablePower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        var masterDeck = PileType.Deck.GetPile(this.Owner);
        CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
            c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
        );

        // --- THE WISDOM FORM FIX ---
        // Calculate the final Vulnerable amount dynamically before the loop
        decimal finalVulnerable = this.DynamicVars.Vulnerable.BaseValue;
        if (this.Owner.HasPower<WisdomFormPower>())
        {
            finalVulnerable += 1; // Adds +1 Vulnerable. Feel free to change to 2 if you want it stronger!
        }

        foreach (var monster in this.CombatState.Enemies)
        {
            if (monster != null && !monster.IsDead)
            {
                // 1. Apply Vulnerable using our dynamically calculated amount
                await PowerCmd.Apply<VulnerablePower>(
                    monster,
                    finalVulnerable,
                    this.Owner.Creature,
                    this
                );

                // 2. The Callback Logic
                if (trueMasterCard is SoraMagicCard magicMasterCard)
                {
                    Func<Task> onDeathAction = async () =>
                    {
                        if (magicMasterCard.Experience < EvolutionRequirement)
                        {
                            magicMasterCard.Experience += 1;

                            if (magicMasterCard.Experience >= EvolutionRequirement)
                            {
                                await this.EvolveIntoMagnera(magicMasterCard);
                            }
                        }
                    };

                    var existingTracker = monster.GetPower<AssistTrackerPower>();

                    if (existingTracker != null)
                    {
                        existingTracker.AddCallback(onDeathAction);
                    }
                    else
                    {
                        var newTracker = await PowerCmd.Apply<AssistTrackerPower>(
                            monster,
                            1m,
                            this.Owner.Creature,
                            this
                        );

                        if (newTracker != null)
                        {
                            newTracker.AddCallback(onDeathAction);
                        }
                    }
                }
            }
        }
    }
    
    public async Task EvolveIntoMagnera(SoraMagicCard masterDeckCard)
    {
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
    
    public override int MaxUpgradeLevel => 0;
}