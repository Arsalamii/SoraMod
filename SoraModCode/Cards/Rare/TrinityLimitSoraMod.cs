using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Rare;

[Pool(typeof(SoraModCardPool))]
public class TrinityLimitSoraMod() : SoraKeybladeCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    // 1. REGISTER THE DAMAGE
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        // 20 Base Damage
        get => new List<DynamicVar> { new DamageVar(20, ValueProp.Move) };
    }

    // 2. THE PLAY SEQUENCE
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // The initial attack
        await PerformTrinityHit(choiceContext, cardPlay);

        // Check the Drive Gauge for the second hit!
        if (this.Owner?.PlayerCombatState != null && this.Owner.PlayerCombatState.Stars >= 3)
        {
            // Consume the 3 Stars! 
            await PlayerCmd.LoseStars(3m, this.Owner.Creature.Player);

            // Add a very slight delay
            await Cmd.Wait(0.3f);

            // The bonus attack
            await PerformTrinityHit(choiceContext, cardPlay);
        }
    }

    // 3. THE HELPER METHOD
    private async Task PerformTrinityHit(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(this.CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);
    }

    // 4. THE UPGRADE
    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(8m);
    }
}