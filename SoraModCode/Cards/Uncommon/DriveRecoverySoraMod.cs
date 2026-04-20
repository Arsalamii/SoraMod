using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SoraMod.SoraModCode.Cards.Uncommon;

public class DriveRecoverySoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new StarsVar(3) };
    }
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] 
    { 
        CardKeyword.Exhaust 
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DriveRecoverySoraMod driveRecoverySoraMod = this;
        await CreatureCmd.TriggerAnim(driveRecoverySoraMod.Owner.Creature, "Cast", driveRecoverySoraMod.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainStars(driveRecoverySoraMod.DynamicVars.Stars.BaseValue, driveRecoverySoraMod.Owner);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Retain);
}