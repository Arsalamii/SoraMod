using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class DarkShieldSoraMod() : SoraModCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag> { CardTag.Defend };
    }

    public override bool GainsBlock => true;

    // 1. SET BASE STATS: 18 Block, 1 Status Card
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new BlockVar(18m, ValueProp.Move),
            new DynamicVar("Status", 1m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DarkShieldSoraMod card = this;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 3. GAIN BASE BLOCK
        await CommonActions.CardBlock(card, cardPlay);

        // 4. ADD WOUND TO HAND
        int statusAmount = (int)card.DynamicVars["Status"].BaseValue;
        
        for (int i = 0; i < statusAmount; i++)
        {
            // We use the exact same logic as Shadow Swarm, but targeting the Hand!
            var generatedCardResult = await CardPileCmd.AddGeneratedCardToCombat(new Wound(), PileType.Hand, true);
            
            // Pop the Wound visually on screen so the player knows what happened
            CardCmd.PreviewCardPileAdd(generatedCardResult, 2.2f);
        }
    }

    // 5. UPGRADE: +4 Block (from 18 to 22)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Block.UpgradeValueBy(4m);
        // Status variable is left alone so it always stays at exactly 1!
    }
}