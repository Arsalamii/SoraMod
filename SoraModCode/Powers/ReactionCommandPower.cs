using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Powers;

// Assuming SoraModPower is your base custom power class!
public class ReactionCommandPower : SoraModPower 
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // 1. Verify this is actually an attack hitting us!
        // We use props.IsPoweredAttack() just like Thorns so we don't counterattack poison or self-damage!
        if (target != this.Owner || dealer == null || !props.IsPoweredAttack() || amount <= 0)
        {
            return;
        }

        // 2. Calculate Block Lost!
        // (Use autocomplete on this.Owner to find the exact property name if it isn't simply .Block. 
        // It might be .CurrentBlock or something similar!)
        decimal blockLost = Math.Min((decimal)this.Owner.Block, amount);

        if (blockLost > 0)
        {
            this.Flash();

            // 3. Counterattack!
            // We use CreatureCmd.Damage directly like Thorns does. This skips Sora doing 
            // a full attack animation, but instantly zaps the enemy with the counter-damage.
            await CreatureCmd.Damage(
                choiceContext, 
                dealer, 
                blockLost, 
                ValueProp.Unpowered, // Unpowered means Strength doesn't buff the counter-damage
                this.Owner, 
                null
            );
        }

        // 4. Remove the buff since it only triggers on the "next" attack!
        // (We put this outside the blockLost check so that if we get hit with 0 Block, 
        // the buff still technically gets "used up" by the attack).
        this.RemoveInternal();
    }

    // Clean up the buff if the enemies refuse to attack us this turn!
    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        this.RemoveInternal();
        return Task.CompletedTask;
    }
}