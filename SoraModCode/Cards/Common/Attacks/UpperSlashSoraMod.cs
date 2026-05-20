using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Common;

namespace SoraMod.SoraModCode.Cards.Common.Attacks;

[Pool(typeof(SoraModCardPool))]
public class UpperSlashSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS: 6 Damage, 3 Upper Slash Power
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(6m, ValueProp.Move),
            new PowerVar<UpperSlashPower>(3m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        UpperSlashSoraMod card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        
        // 2. DEAL DAMAGE
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard((CardModel) card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 3. APPLY THE POWER
        // Pull the exact amount (3) from our PowerVar!
        var buffAmount = card.DynamicVars.Select(v => v.Value).OfType<PowerVar<UpperSlashPower>>().First().BaseValue;

        await PowerCmd.Apply<UpperSlashPower>(
            this.Owner.Creature, 
            buffAmount, 
            this.Owner.Creature, 
            this
        );
    }
    
    // 4. UPGRADE: +3 Damage (to 9) and +1 Power (to 4)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(3m);
        
        var buffVar = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<UpperSlashPower>>().First();
        buffVar.UpgradeValueBy(1m);
    }
}