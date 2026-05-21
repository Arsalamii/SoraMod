using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SoraMod.SoraModCode.Cards.Common.Attacks;

[Pool(typeof(SoraModCardPool))]
public class AerialSweepSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS: 6 Damage and 1 Weak
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(6m, ValueProp.Move),
            new PowerVar<WeakPower>(1m)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AerialSweepSoraMod card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        
        // 2. DEAL DAMAGE
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard((CardModel) card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 3. CHECK IF THE ENEMY INTENDS TO ATTACK
        bool isAttacking = false;
        
        if (cardPlay.Target.Monster != null)
        {
            isAttacking = cardPlay.Target.Monster.IntendsToAttack; 
        }

        // 4. APPLY WEAK IF TRUE
        if (isAttacking)
        {
            // We use LINQ to search the values and grab the first PowerVar<WeakPower> we find!
            var weakAmount = card.DynamicVars.Select(v => v.Value).OfType<PowerVar<WeakPower>>().First().BaseValue;

            await PowerCmd.Apply<WeakPower>(
                cardPlay.Target, 
                weakAmount, 
                this.Owner.Creature, 
                this
            );
        }
    }
    
    // 5. UPGRADE: +3 Damage and +1 Weak
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(3m);
    
        // Find it and upgrade it!
        var weakVar = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<WeakPower>>().First();
        weakVar.UpgradeValueBy(1m);
    }
}