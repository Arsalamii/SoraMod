using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class SoraReflectPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => false; 

    // 1. THE NEW DAMAGE HOOK (Stolen directly from Flame Barrier!)
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // Check if Sora was the one hit, if there's actually an attacker, and if it was a real attack (not poison/thorns)
        if (target != this.Owner || dealer == null || !props.IsPoweredAttack())
            return;

        // --- IDE CHECK REQUIRED HERE ---
        // If 'DamageBlocked' gives a red squiggle, delete it, type 'result.', 
        // and look for things like 'Blocked', 'BlockDamage', or 'AmountBlocked' in the autocomplete!
        decimal blockLost = result.BlockedDamage; 

        if (blockLost > 0)
        {
            // Fire the blocked damage directly back at the attacker as unmitigated Thorns-style damage!
            await CreatureCmd.Damage(
                choiceContext, 
                dealer, 
                blockLost, 
                ValueProp.Unpowered, 
                this.Owner, 
                null
            );
        }
    }

    // 2. THE NEW EXPIRATION HOOK
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // Just like Flame Barrier, it stays active during the enemy's turn, 
        // and ONLY wears off when the ENEMY turn ends (meaning Sora's turn is starting)
        if (this.Owner.Side == side)
            return;
            
        await PowerCmd.Remove(this);
    }
}