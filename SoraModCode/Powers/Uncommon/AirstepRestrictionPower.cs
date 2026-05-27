using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class AirstepRestrictionPower : SoraModPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => false; 

    // THE RESTRICTION HOOK: Stolen straight from Velvet Choker!
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        // If it belongs to us AND it has the Magic tag, block it from being played!
        if (card.Owner.Creature == this.Owner && card.Tags.Contains(SoraModEnums.Magic))
        {
            return false;
        }

        return true;
    }

    // THE EXPIRATION HOOK: Remove the lock at the end of the turn
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == this.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
        await base.BeforeTurnEnd(choiceContext, side);
    }
}