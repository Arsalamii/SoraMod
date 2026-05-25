using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models; 
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class ThunderSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 5;

    public ThunderSoraMod() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
    }

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip> 
    { 
        HoverTipFactory.Static(StaticHoverTip.Fatal) 
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(6m, ValueProp.Move),
        new RepeatVar(2) // 2 Hits!
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Attack", this.Owner.Character.CastAnimDelay);

        // 1. SNAPSHOT ALIVE ENEMIES VALID FOR FATAL
        // We grab every enemy that is currently alive AND is allowed to trigger fatal rewards
        var validEnemiesBefore = this.CombatState.Enemies
            .Where(e => e != null && !e.IsDead && e.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
            .ToList();

        // 2. DEAL MULTI-HIT RANDOM DAMAGE
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitCount(this.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(this.CombatState)
            .Execute(choiceContext);

        // 3. CHECK WHO DIED
        // We just count how many of those specific enemies are now dead!
        int fatalKillsThisPlay = validEnemiesBefore.Count(e => e.IsDead || e.CurrentHp <= 0);

        // 4. GRANT EXP AND EVOLVE
        if (fatalKillsThisPlay > 0)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => c.Id == this.Id);

            if (trueMasterCard is SoraMagicCard magicMasterCard)
            {
                magicMasterCard.Experience += fatalKillsThisPlay;

                if (magicMasterCard.Experience >= EvolutionRequirement)
                {
                    await this.EvolveIntoThundara(magicMasterCard);
                }
            }
        }
    }

    private async Task EvolveIntoThundara(SoraMagicCard masterDeckCard)
    {
        var newThundara = this.CardScope.CreateCard<ThundaraSoraMod>(this.Owner);
        newThundara.Experience = masterDeckCard.Experience; 

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newThundara); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<ThundaraSoraMod>(this);
        }
    }
}