using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Cards.Common;

namespace SoraMod.SoraModCode.Powers;

public class WisdomFormPower : SoraModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 1. SKILL CARD DRAW EFFECT
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Check if it's a skill AND make sure it is NOT the Wisdom Form card itself!
        if (cardPlay.Card.Type == CardType.Skill && !(cardPlay.Card is WisdomFormSoraMod) && !(cardPlay.Card is RevertSoraMod))
        {
            await CardPileCmd.Draw(context, 1, this.Owner.Player);
        }
        
        await base.AfterCardPlayed(context, cardPlay);
    }

    // 2. MAGIC DAMAGE SCALING (For cards like Fire)
    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Magic))
        {
            return 2m; // Add 2 Damage!
        }
        return 0m;
    }

    // 3. MAGIC BLOCK SCALING (For cards like Aero)
    // NOTE: Use autocomplete on `public override ` to find the exact Block hook!
    // It will likely be ModifyBlockAdditive or ModifyShieldAdditive
    
    public override decimal ModifyBlockAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource?.Owner?.Creature == this.Owner && cardSource != null && cardSource.Tags.Contains(SoraModEnums.Magic))
        {
            return 2m; // Add 2 Block!
        }
        return 0m;
    }

    // 4. DRIVE GAUGE & REVERT CLEANUP (Identical to Valor!)
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != this.Owner.Side)
        {
            return;
        }
        
        if (this.Owner.Player.PlayerCombatState.Stars > 0)
        {
            this.Owner.Player.PlayerCombatState.LoseStars(1m);
        }

        if (this.Owner.Player.PlayerCombatState.Stars <= 0)
        {
            var revertCard = this.Owner.Player.PlayerCombatState.AllCards.FirstOrDefault(c => c is RevertSoraMod);
            if (revertCard != null)
            {
                await CardCmd.Exhaust(choiceContext, revertCard);
            }
            this.RemoveInternal();
        }
    }
}