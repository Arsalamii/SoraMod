using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Forms;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class MagneraSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public MagneraSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<VulnerablePower>(2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        var masterDeck = PileType.Deck.GetPile(this.Owner);
        
        // APPLIED FIX: Using MagicSerialNumber to safely identify the correct master card!
        CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
            c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
        );

        // --- WISDOM FORM CHECK ---
        decimal finalVulnerable = this.DynamicVars.Vulnerable.BaseValue;
        if (this.Owner.HasPower<WisdomFormPower>())
        {
            finalVulnerable += 2;
        }

        foreach (var monster in this.CombatState.Enemies)
        {
            if (monster != null && !monster.IsDead)
            {
                // 1. Apply Vulnerable using finalVulnerable
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
                                await this.EvolveIntoMagnega(magicMasterCard);
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

    public async Task EvolveIntoMagnega(SoraMagicCard masterDeckCard)
    {
        var newMagnega = this.CardScope.CreateCard<MagnegaSoraMod>(this.Owner);
        if (this.IsUpgraded)
        {
            newMagnega.UpgradeInternal();
            newMagnega.FinalizeUpgradeInternal();
        }

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newMagnega); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<MagnegaSoraMod>(this);
        }
    }
    
    public override int MaxUpgradeLevel => 0;
}