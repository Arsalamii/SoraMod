using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SoraMod.SoraModCode.Powers.Common;

  
public class RetaliatePower : SoraModPower 
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => false; 
    
    // 1. MODIFY THE DAMAGE
    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Keyblade))
        {
            return this.Amount; 
        }
        
        return 0m;
    }

    // 2. REMOVE AFTER PLAYING A KEYBLADE
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            this.RemoveInternal();
        }
        
        await base.AfterCardPlayed(context, cardPlay);
    }

    // 3. EXPIRE AT THE END OF THE TURN
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == this.Owner.Side)
        {
            this.RemoveInternal();
        }
        
        await base.AfterTurnEnd(choiceContext, side);
    }
}