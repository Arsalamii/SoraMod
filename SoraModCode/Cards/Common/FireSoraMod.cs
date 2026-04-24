using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models; // Needed for ModelDb
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
                // Snapshot HP before the attack!
                int hpBefore = monster.CurrentHp; // (Try CurrentHp if Hp throws an error)

                await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(monster)
                    .Execute(choiceContext);

                // Bulletproof Kill Check: Is it flagged as dead, OR did its HP hit 0?
                if (monster.IsDead || (hpBefore > 0 && monster.CurrentHp <= 0))
                {
                    totalKillsThisPlay++;
                }
            }
        }

        // Grant EXP and check for Evolution!
        if (totalKillsThisPlay > 0)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => c.Id == this.Id);

            if (trueMasterCard is SoraMagicCard magicMasterCard)
            {
                magicMasterCard.Experience += totalKillsThisPlay;

                if (magicMasterCard.Experience >= EvolutionRequirement)
                {
                    await this.EvolveIntoFira(magicMasterCard);
                }
            }
        }
    }

    private async Task EvolveIntoFira(SoraMagicCard masterDeckCard)
    {
        // 1. Create the brand new Fira Card directly
        var newFira = this.CardScope.CreateCard<FiraSoraMod>(this.Owner);
        if (this.IsUpgraded)
        {
            newFira.UpgradeInternal();
            newFira.FinalizeUpgradeInternal();
        }

        // 2. THE FIX: Safely swap it in the Master Deck (Bypasses the IsEnding block!)
        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                // Yank the old Fire out, and put the new Fira in!
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newFira); 
            }
        }

        // 3. Swap the temporary combat card in your hand (Only if combat isn't fading to black!)
        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<FiraSoraMod>(this);
        }
    }
}