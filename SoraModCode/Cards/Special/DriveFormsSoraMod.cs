using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Cards.Special;

public class DriveFormsSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 1. The Keywords (Innate, Retain, Exhaust)
    public override IEnumerable<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Innate,
        CardKeyword.Retain,
        CardKeyword.Exhaust
    };

    // Check if Sora has 3 or more stars
    protected override bool IsPlayable
    {
        get
        {
            return (this.Owner?.PlayerCombatState?.Stars ?? 0) >= 3;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. Ask the engine to properly forge the temporary forms for the menu!
        var valorChoice = this.CombatState.CreateCard<ValorFormSoraMod>(this.Owner); 
        var wisdomChoice = this.CombatState.CreateCard<WisdomFormSoraMod>(this.Owner);

        valorChoice.SetToFreeThisCombat();
        wisdomChoice.SetToFreeThisCombat();

        // 2. Put them in an IEnumerable/List for the engine
        List<CardModel> formChoices = new List<CardModel> { valorChoice, wisdomChoice };

        // 3. Pop the menu open and wait for the player to click one
        CardModel? selectedCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, formChoices, this.Owner);

        // 4. If the player selected a form, transform!
        if (selectedCard != null)
        {
            await CardCmd.AutoPlay(choiceContext, selectedCard, this.Owner.Creature);
        }
    }
}