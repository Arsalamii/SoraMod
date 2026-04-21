using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Powers;
using System.Linq;

namespace SoraMod.SoraModCode.Cards.Special;

public class RevertSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // The native engine WILL read this and keep it in your hand!
    public override IEnumerable<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Retain,
        CardKeyword.Exhaust
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var valorPower = this.Owner.Creature.Powers.FirstOrDefault(p => p is ValorFormPower);
        if (valorPower != null)
        {
            valorPower.RemoveInternal(); 
        }
    }
    
}