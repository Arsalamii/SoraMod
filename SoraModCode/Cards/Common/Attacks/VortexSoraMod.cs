using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Attacks;

[Pool(typeof(SoraModCardPool))]
public class VortexSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) 
{
    // 1. SET BASE STATS: 5 Damage
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(5m, ValueProp.Move) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 2. LOOP THROUGH ALL ENEMIES
        // We use the exact loop structure from your FireSoraMod
        foreach (var monster in this.CombatState.Enemies.ToList())
        {
            if (monster != null && !monster.IsDead)
            {
                await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(monster)
                    .Execute(choiceContext);
            }
        }
    }
    
    // 3. UPGRADE: +3 Damage (From 5 to 8)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}