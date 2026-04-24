using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.DynamicVars;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class CuragaSoraMod : SoraMagicCard
{
    public CuragaSoraMod() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    // Use your awesome HealVar!
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new SoraHealVar(12) 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. Heal Sora! No EXP tracking needed for the final form.
        await CreatureCmd.Heal(this.Owner.Creature, this.DynamicVars.Heal.BaseValue);
    }
}