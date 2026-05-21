using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class SynthesisSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. SET BASE STATS: 2 Draw
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DynamicVar("Draw", 2m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SynthesisSoraMod card = this;
        
        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 3. PROMPT THE PLAYER TO DISCARD
        var selectedCards = await CardSelectCmd.FromHandForDiscard(
            choiceContext, 
            card.Owner, // Acrobatics uses 'Owner' directly here for the UI prompt
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), 
            null, 
            card
        );

        // Grab the single card the player clicked
        var cardToDiscard = selectedCards.FirstOrDefault();

        // 4. ACTUALLY DISCARD IT
        if (cardToDiscard != null)
        {
            await CardCmd.Discard(choiceContext, cardToDiscard);
        }

        // 5. DRAW CARDS
        int drawAmount = (int)card.DynamicVars["Draw"].BaseValue;
        await CardPileCmd.Draw(choiceContext, drawAmount, card.Owner.Creature.Player);
    }

    // 6. UPGRADE: +1 Draw
    protected override void OnUpgrade() 
    {
        this.DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}