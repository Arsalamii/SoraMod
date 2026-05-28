using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class DriveConverterSoraMod : SoraModCard
{
    // Cost 0, Skill, Token Rarity
    public DriveConverterSoraMod() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DynamicVar("Stars", 1m) 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Gain 1 Drive!
        await PlayerCmd.GainStars(this.DynamicVars["Stars"].BaseValue, this.Owner);
    }

    public override int MaxUpgradeLevel => 0; 
}