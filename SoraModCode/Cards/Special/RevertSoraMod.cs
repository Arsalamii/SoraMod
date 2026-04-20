using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Special;

public class RevertSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] 
    { 
        CardKeyword.Retain, 
        CardKeyword.Exhaust 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. Find the Valor Form power on Sora
        var valorPower = this.Owner.Creature.Powers.FirstOrDefault(p => p is ValorFormPower);
        
        if (valorPower != null)
        {
            // 2. Remove it safely! 
            valorPower.RemoveInternal(); 
        }
    }
}