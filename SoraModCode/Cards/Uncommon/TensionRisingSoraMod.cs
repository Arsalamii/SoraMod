using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon;

[Pool(typeof(SoraModCardPool))]
public class TensionRisingSoraMod() : SoraModCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar>
        {
            new CalculationBaseVar(0m), 
            new ExtraDamageVar(1m),     
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, target) => 
            {
                if (card.Owner?.PlayerCombatState != null)
                {
                    return card.Owner.PlayerCombatState.Stars;
                }
                return 0m;
            }),
            // ADD THIS: Register the Stars variable! (Base value of 1)
            new StarsVar(1) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash") 
            .Execute(choiceContext);

        // CHANGE THIS: Tell it to read the dynamic variable instead of the hardcoded 1m!
        await PlayerCmd.GainStars(this.DynamicVars.Stars.BaseValue, this.Owner.Creature.Player); 
    }
    
    // Now this will work perfectly!
    protected override void OnUpgrade() => this.DynamicVars.Stars.UpgradeValueBy(2M);
}