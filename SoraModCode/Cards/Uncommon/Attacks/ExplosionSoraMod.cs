using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class ExplosionSoraMod() : SoraKeybladeCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    // 1. SET BASE STATS: 8 Base Damage, 4 Scaling Damage
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(8m, ValueProp.Move),
            new DynamicVar("Scale", 4m)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExplosionSoraMod card = this;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 3. COUNT PREVIOUS KEYBLADES
        // We check the history for finished cards this turn that have the Keyblade tag
        int keybladesPlayed = CombatManager.Instance.History.CardPlaysFinished.Count(e => 
            e.HappenedThisTurn(this.CombatState) && 
            e.CardPlay.Card.Owner == this.Owner && 
            e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade));

        // 4. CALCULATE TOTAL DAMAGE
        decimal scaleAmount = card.DynamicVars["Scale"].BaseValue;
        decimal totalDamage = card.DynamicVars.Damage.BaseValue + (keybladesPlayed * scaleAmount);

        // 5. DEAL AOE DAMAGE
        // We feed the dynamically calculated totalDamage directly into the builder!
        await DamageCmd.Attack(totalDamage)
            .FromCard(card)
            .TargetingAllOpponents(card.CombatState)
            .Execute(choiceContext);
    }

    // 6. UPGRADE: +4 Base Damage (from 8 to 12)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(4m);
        // The Scale variable remains untouched so it always gives +4 per Keyblade!
    }
}