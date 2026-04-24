using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.DynamicVars;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Rare;

[Pool(typeof(SoraModCardPool))]
public class CureSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public CureSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 1. Give the card the Exhaust keyword!
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    // 2. Set the base healing amount
    // (Note: If MagicVar throws a red line, try MagicNumberVar or HealVar depending on your BaseLib!)
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new SoraHealVar(5) 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. Grab the base healing amount
        int finalHealAmount = (int)this.DynamicVars.Heal.BaseValue;

        // 2. Check if we are in a Drive Form! (Make sure to include your Powers using statement at the top)
        bool inDriveForm = this.Owner.HasPower<WisdomFormPower>();

        if (inDriveForm)
        {
            finalHealAmount += 3; // Give a +3 bonus! (Change this to whatever number you like)
        }

        // 3. Execute the Heal with our new calculated amount!
        await CreatureCmd.Heal(this.Owner.Creature, finalHealAmount);

        // 4. Grant EXP!
        var masterDeck = PileType.Deck.GetPile(this.Owner);
        CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => c.Id == this.Id);

        if (trueMasterCard is SoraMagicCard magicMasterCard)
        {
            magicMasterCard.Experience += 1;

            if (magicMasterCard.Experience >= EvolutionRequirement)
            {
                await this.EvolveIntoCura(magicMasterCard);
            }
        }
    }

    private async Task EvolveIntoCura(SoraMagicCard masterDeckCard)
    {
        // Exact same bulletproof manual swap we used for Fire!
        var newCura = this.CardScope.CreateCard<CuraSoraMod>(this.Owner);
        if (this.IsUpgraded)
        {
            newCura.UpgradeInternal();
            newCura.FinalizeUpgradeInternal();
        }

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newCura); 
            }
        }
    }
}