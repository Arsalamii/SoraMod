using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class RippleDriveSoraMod : SoraModCard
{
    public RippleDriveSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 1. SET BASE STATS: 3 Block, 2 Scaling Block
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(3m, ValueProp.Move),
        new DynamicVar("Scale", 2m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        // 3. COUNT PREVIOUS KEYBLADES
        int keybladesPlayed = CombatManager.Instance.History.CardPlaysFinished.Count(e => 
            e.HappenedThisTurn(this.CombatState) && 
            e.CardPlay.Card.Owner == this.Owner && 
            e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade));

        // 4. CALCULATE TOTAL BLOCK
        decimal originalBaseBlock = this.DynamicVars.Block.BaseValue;
        decimal scaleAmount = this.DynamicVars["Scale"].BaseValue;
        decimal totalBlock = originalBaseBlock + (keybladesPlayed * scaleAmount);

        // 5. APPLY DYNAMIC BLOCK SAFELY
        // We temporarily boost the block value on the card so the native CardBlock helper 
        // applies the correct amount (and triggers the correct visual numbers/relics), 
        // then we immediately restore it!
        this.DynamicVars.Block.BaseValue = totalBlock;
        
        await CommonActions.CardBlock(this, cardPlay);
        
        this.DynamicVars.Block.BaseValue = originalBaseBlock;
    }

    // 6. UPGRADE: +1 Base Block (from 3 to 4)
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(1m);
    }
}