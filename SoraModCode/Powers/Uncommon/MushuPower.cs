using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class MushuPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; // Counter means it's a Turn Timer!

    // Virtual damage property so we can easily swap it for the upgrade
    protected virtual decimal DamageAmount => 2m;

    // 1. THE DAMAGE HOOK
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // FIX 1: Match Player to Player
        if (cardPlay.Card.Owner == this.Owner.Player && cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            var aliveEnemies = this.Owner.CombatState.Enemies.Where(e => !e.IsDead).ToList();
        
            if (aliveEnemies.Count > 0)
            {
                // FIX 2: Route through Player to get the RunState RNG, and pick the 'Misc' generator
                var rng = this.Owner.Player.RunState.Rng.CombatTargets; 

                // Now that 'rng' is a specific generator, .Next() will work flawlessly!
                var randomEnemy = aliveEnemies[rng.NextInt(aliveEnemies.Count)];

                // FIX 3: Removed .FromPower(this) entirely
                await DamageCmd.Attack(this.DamageAmount)
                    .Targeting(randomEnemy)
                    .Execute(choiceContext);
            }
        }
    
        await base.AfterCardPlayed(choiceContext, cardPlay);
    }

    // 2. THE TIMER HOOK
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == this.Owner.Side)
        {
            // Tick the turns down! (Will auto-remove when hitting 0)
            await PowerCmd.Decrement(this);
        }
        await base.BeforeSideTurnStart(choiceContext, side, combatState);
    }
}

// THE UPGRADED POWER
// Because it inherits from MushuPower, it gets the Keyblade checking and the Turn Timer for free!
public class MushuUpgradedPower : MushuPower
{
    protected override decimal DamageAmount => 3m;
}