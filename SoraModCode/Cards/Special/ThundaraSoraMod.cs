using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models; 
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class ThundaraSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 10;

    public ThundaraSoraMod() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
    }

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip> 
    { 
        HoverTipFactory.Static(StaticHoverTip.Fatal) 
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(8m, ValueProp.Move),
        new RepeatVar(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Attack", this.Owner.Character.CastAnimDelay);

        // 1. SNAPSHOT ALIVE ENEMIES
        var validEnemiesBefore = this.CombatState.Enemies
            .Where(e => e != null && !e.IsDead && e.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
            .ToList();

        // 2. DEAL DAMAGE
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitCount(this.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(this.CombatState)
            .Execute(choiceContext);

        // 3. COUNT KILLS
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
                    await this.EvolveIntoThundaga(magicMasterCard);
                }
            }
        }
    }

    private async Task EvolveIntoThundaga(SoraMagicCard masterDeckCard)
    {
        var newThundaga = this.CardScope.CreateCard<ThundagaSoraMod>(this.Owner);
        newThundaga.Experience = masterDeckCard.Experience;

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newThundaga); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<ThundagaSoraMod>(this);
        }
    }
}