using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class DarkVolleySoraMod : SoraMagicCard
{
    public DarkVolleySoraMod() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
    }

    // 1. SET BASE STATS: 4 Damage
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> 
    { 
        new DamageVar(4m, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // 3. LOCATE THE PILES
        var drawPile = this.Owner.Creature.Player.Piles.FirstOrDefault(p => p.Type == PileType.Draw);
        var discardPile = this.Owner.Creature.Player.Piles.FirstOrDefault(p => p.Type == PileType.Discard);

        int totalGunkCards = 0;

        // 4. COUNT STATUSES AND CURSES
        if (drawPile != null)
        {
            totalGunkCards += drawPile.Cards.Count(c => c.Type == CardType.Status || c.Type == CardType.Curse);
        }

        if (discardPile != null)
        {
            totalGunkCards += discardPile.Cards.Count(c => c.Type == CardType.Status || c.Type == CardType.Curse);
        }

        // 5. FIRE THE VOLLEY
        // We only run the attack if there's actually at least 1 hit to process
        if (totalGunkCards > 0)
        {
            await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                .WithHitCount(totalGunkCards)
                .FromCard(this)
                .TargetingRandomOpponents(this.CombatState)
                .Execute(choiceContext);
        }
    }

    // 6. UPGRADE: +1 Damage per hit (from 4 to 5)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}