using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class GuardBreakSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS: 8 Damage
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(8m, ValueProp.Move)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        GuardBreakSoraMod card = this;
        
        if (cardPlay.Target == null) return;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 3. REMOVE ENEMY BLOCK
        // We first check if they even have block to remove
        if (cardPlay.Target.Block > 0)
        {
            // We use LoseBlock and feed it the enemy's exact current block amount!
            await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);
        }

        // 4. DEAL DAMAGE
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    // 5. UPGRADE: +4 Damage (from 8 to 12)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}