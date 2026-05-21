using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Common;

namespace SoraMod.SoraModCode.Cards.Basic;

[Pool(typeof(SoraModCardPool))]
public class RetaliateSoraMod() : SoraModCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag> { CardTag.Defend };
    }

    public override bool GainsBlock => true;

    // 1. SET BASE STATS: 5 Block, 4 Retaliate Power
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new BlockVar(5m, ValueProp.Move),
            new PowerVar<RetaliatePower>(4m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        RetaliateSoraMod card = this;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);
        
        // 3. GAIN BLOCK
        await CommonActions.CardBlock(this, cardPlay);

        // 4. APPLY THE POWER
        var buffAmount = card.DynamicVars.Select(v => v.Value).OfType<PowerVar<RetaliatePower>>().First().BaseValue;

        await PowerCmd.Apply<RetaliatePower>(
            this.Owner.Creature, 
            buffAmount, 
            this.Owner.Creature, 
            this
        );
    }
    
    // 5. UPGRADE: +3 Block (to 8) and +2 Power (to 6)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Block.UpgradeValueBy(3m);
        
        var buffVar = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<RetaliatePower>>().First();
        buffVar.UpgradeValueBy(2m);
    }
}