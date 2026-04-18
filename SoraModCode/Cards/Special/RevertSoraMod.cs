using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Special;

// No [Pool] attribute here! It's a special token card.
public class RevertSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // Make the card naturally Retain (stay in hand) and Exhaust (vanish on play)
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] 
    { 
        CardKeyword.Retain, 
        CardKeyword.Exhaust 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. REMOVE THE VALOR FORM BUFF HERE
        // e.g., await RemoveValorFormPower(cardPlay.Target);
    }
}