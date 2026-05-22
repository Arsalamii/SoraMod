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
public class SonicBladeSoraMod() : SoraKeybladeCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(4m, ValueProp.Move), // Base Damage
            new DynamicVar("Combo", 4m),       // Combo Damage
            new DynamicVar("Hits", 3m),        // Base Hits
            new DynamicVar("ComboHits", 4m)    // Combo Hits
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SonicBladeSoraMod card = this;
        
        if (cardPlay.Target == null) return;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 3. CHECK THE COMBO CONDITION
        // We look at the history of finished cards this turn and grab the very last one.
        var lastCardEntry = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(e => 
                e.HappenedThisTurn(this.CombatState) && 
                e.CardPlay.Card.Owner == this.Owner);

        // Check if that last card exists AND has the Keyblade tag
        bool isComboActive = lastCardEntry != null && 
                             lastCardEntry.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade);

        // 4. DETERMINE DAMAGE AND HITS
        decimal damageToDeal = isComboActive ? card.DynamicVars["Combo"].BaseValue : card.DynamicVars.Damage.BaseValue;
        int hits = isComboActive ? (int)card.DynamicVars["ComboHits"].BaseValue : (int)card.DynamicVars["Hits"].BaseValue;

        // 5. DEAL MULTI-HIT DAMAGE
        for (int i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(damageToDeal)
                .FromCard(card)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }
    }

    // 6. UPGRADE LOGIC
    // Base damage +1 (from 4 to 5).
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}