using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class FlashStepSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 2. SET BASE STATS: 8 Damage, 1 Power Stack
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(8m, ValueProp.Move),
            new PowerVar<FlashStepPower>(1m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FlashStepSoraMod card = this;
        
        if (cardPlay.Target == null) return;

        // 3. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 4. DEAL DAMAGE
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 5. APPLY THE COST REDUCTION POWER
        var buffAmount = card.DynamicVars.Select(v => v.Value).OfType<PowerVar<FlashStepPower>>().First().BaseValue;

        await PowerCmd.Apply<FlashStepPower>(
            card.Owner.Creature, 
            buffAmount, 
            card.Owner.Creature, 
            card
        );
    }

    // 6. UPGRADE: +3 Damage (from 8 to 11)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}