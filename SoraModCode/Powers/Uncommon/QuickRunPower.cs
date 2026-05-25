using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class QuickRunPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    // Counter stack type lets the player play Quick Run twice to make their next TWO Keyblades cost 1 less!
    public override PowerStackType StackType => PowerStackType.Counter; 

    // 1. THE COST MODIFIER HOOK
    // (If this gives a red squiggle, type 'public override ' and let your IDE 
    // autocomplete show you the exact name STS2 uses for modifying Energy Cost!)
    public override CardEneR
    public override decimal ModifyEnergyCostAdditive(CardModel card, decimal currentCost)
    {
        // If the card in our hand is a Keyblade, tell the engine it costs 1 less!
        if (card.Tags.Contains(SoraModEnums.Keyblade))
        {
            return -1m; 
        }
        return 0m;
    }

    // 2. THE CONSUMPTION HOOK
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // If we successfully played a Keyblade, we use up one stack of Quick Run!
        if (cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            this.Amount -= 1;
            
            // If we are out of charges, remove the power completely
            if (this.Amount <= 0)
            {
                await PowerCmd.Remove(this);
            }
        }
        await base.AfterCardPlayed(choiceContext, cardPlay);
    }

    // 3. THE EXPIRATION HOOK
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // "This turn" means we wipe the buff when Sora ends his turn, even if he didn't use it.
        if (side == this.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
        await base.BeforeTurnEnd(choiceContext, side);
    }
}