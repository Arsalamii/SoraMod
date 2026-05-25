using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;


namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class EsunaSoraMod : SoraMagicCard
{
    public EsunaSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // Show the Exhaust keyword on hover since this card exhausts things!
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>
    {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };

    // 1. SET BASE STATS: 7 Block
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(7m, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 2. PLAY ANIMATION & GAIN BLOCK
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, cardPlay);

        // 3. CHECK IF WE HAVE VALID CARDS FIRST
        var hand = this.Owner.Creature.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        
        if (hand != null && hand.Cards.Any(c => c.Type == CardType.Status || c.Type == CardType.Curse))
        {
            // 4. SETUP UI PREFERENCES
            // Note: If you want to strictly enforce "Up to 1" (allowing them to cancel), 
            // you might be able to add a property here like `prefs.CanCancel = true;` 
            // depending on what CardSelectorPrefs exposes!
            CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            
            // 5. CREATE OUR FILTER (Only allow Statuses and Curses)
            Func<CardModel, bool> gunkFilter = c => c.Type == CardType.Status || c.Type == CardType.Curse;

            // 6. POP THE SELECTION MENU
            var selectedCards = await CardSelectCmd.FromHand(
                choiceContext, 
                this.Owner, 
                prefs, 
                gunkFilter, 
                this
            );

            // 7. EXHAUST THE CHOSEN CARD
            var cardToExhaust = selectedCards.FirstOrDefault();
            if (cardToExhaust != null)
            {
                await CardCmd.Exhaust(choiceContext, cardToExhaust);
            }
        }
    }

    // 8. UPGRADE: +3 Block (from 7 to 10)
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3m);
    }
}