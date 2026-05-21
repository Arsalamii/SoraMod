using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class FocusSoraMod() : SoraModCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. SET BASE STATS: 7 Block and 2 Drive (Stars)
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new BlockVar(7m, ValueProp.Move), 
            new StarsVar(2)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FocusSoraMod card = this;

        // 1. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 2. GAIN BLOCK
        // Using the exact logic from your GuardSoraMod!
        await CommonActions.CardBlock(this, cardPlay);

        // 3. GAIN DRIVE (STARS)
        await PlayerCmd.GainStars(card.DynamicVars.Stars.BaseValue, card.Owner);
    }

    // 5. UPGRADE: +3 Block and +1 Drive
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Block.UpgradeValueBy(3m);
        this.DynamicVars.Stars.UpgradeValueBy(1m); 
    }
}