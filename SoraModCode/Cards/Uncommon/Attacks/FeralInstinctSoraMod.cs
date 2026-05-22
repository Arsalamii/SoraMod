using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class FeralInstinctSoraMod() : SoraKeybladeCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 2. SET BASE STATS: 4 Base Damage, 10 Feral Damage
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(4m, ValueProp.Move),
            new DynamicVar("Feral", 10m)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FeralInstinctSoraMod card = this;
        
        if (cardPlay.Target == null) return;

        // 3. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 4. CHECK FOR DEFEND TAGS IN HAND
        var hand = card.Owner.Creature.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        bool hasDefendCard = false;

        if (hand != null)
        {
            // 5. CHANGED TO LOOK FOR CardTag.Defend
            hasDefendCard = hand.Cards.Any(c => c.Tags.Contains(CardTag.Defend));
        }

        // 6. DETERMINE DAMAGE
        // If there are NO Defend cards in hand, we use the Feral variable.
        decimal damageToDeal = !hasDefendCard ? card.DynamicVars["Feral"].BaseValue : card.DynamicVars.Damage.BaseValue;

        // 7. DEAL DAMAGE
        await DamageCmd.Attack(damageToDeal)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    // 8. UPGRADE: +2 Base Damage, +4 Feral Damage
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(2m);
        this.DynamicVars["Feral"].UpgradeValueBy(4m);
    }
}