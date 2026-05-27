using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class MastersGuidanceSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public MastersGuidanceSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. SET BASE STATS: 1 Magic (Used to represent the 1 EXP gain)
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DynamicVar("Magic", 1m) 
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // 1. Grab the current draw pile
        var drawPile = PileType.Draw.GetPile(this.Owner);
        
        // 2. Filter it to ONLY include cards with the Magic tag
        var magicCardsInDrawPile = drawPile.Cards
            .Where(c => c.Tags.Contains(SoraModEnums.Magic))
            .ToList();

        // 3. Only open the selection screen if there is actually a Magic card to find
        if (magicCardsInDrawPile.Count > 0)
        {
            // Use the native STS2 grid selection for existing cards (Stolen from SeekerStrike!)
            var selectedCards = await CardSelectCmd.FromSimpleGrid(
                choiceContext, 
                magicCardsInDrawPile, 
                this.Owner, 
                new CardSelectorPrefs(this.SelectionScreenPrompt, 1)
            );

            var selectedCard = selectedCards.FirstOrDefault();

            if (selectedCard != null)
            {
                // 4. THE FIX: Safely move it to the hand!
                await CardPileCmd.Add(selectedCard, PileType.Hand);

                // 5. THE EXP INJECTION
                if (selectedCard is SoraMagicCard drawnMagicCard)
                {
                    int expAmount = (int)this.DynamicVars["Magic"].BaseValue;
                    
                    // Give the combat copy the EXP
                    drawnMagicCard.Experience += expAmount;

                    // Safely find the Master Deck copy using your verified MagicSerialNumber logic
                    var masterDeck = PileType.Deck.GetPile(this.Owner);
                    CardModel trueMasterCard = drawnMagicCard.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
                        c is SoraMagicCard smc && smc.MagicSerialNumber == drawnMagicCard.MagicSerialNumber
                    );

                    // Give the permanent Master Deck copy the EXP too!
                    if (trueMasterCard is SoraMagicCard masterMagicCard)
                    {
                        masterMagicCard.Experience += expAmount;
                    }
                }
            }
        }
    }

    // 4. THE UPGRADE (Cost becomes 0)
    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}