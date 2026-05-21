using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class EtherDropSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. ADD RETAIN AND EXHAUST KEYWORDS
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] 
    { 
        CardKeyword.Retain, 
        CardKeyword.Exhaust 
    };

    // 2. SET BASE STATS: 2 Ether Power
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new PowerVar<EtherPower>(2m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        EtherDropSoraMod card = this;

        // PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // APPLY THE POWER
        var buffAmount = card.DynamicVars.Select(v => v.Value).OfType<PowerVar<EtherPower>>().First().BaseValue;

        await PowerCmd.Apply<EtherPower>(
            this.Owner.Creature, 
            buffAmount, 
            this.Owner.Creature, 
            this
        );
    }

    // 3. UPGRADE: +1 Power
    protected override void OnUpgrade() 
    {
        var buffVar = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<EtherPower>>().First();
        buffVar.UpgradeValueBy(1m);
    }
}