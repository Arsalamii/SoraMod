using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Powers;

[Pool(typeof(SoraModCardPool))]
public class SummonMushuSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public SummonMushuSoraMod() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. BASE STATS: 3 Turns
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DynamicVar("Turns", 3m)
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Buff", this.Owner.Character.CastAnimDelay);

        decimal turns = this.DynamicVars["Turns"].BaseValue;
        
        // Apply the correct power based on the upgrade status!
        if (this.IsUpgraded)
        {
            await PowerCmd.Apply<MushuUpgradedPower>(this.Owner.Creature, turns, this.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<MushuPower>(this.Owner.Creature, turns, this.Owner.Creature, this);
        }
    }

    // 4. THE UPGRADE
    protected override void OnUpgrade()
    {
        // We don't need to change any stats here, because this.IsUpgraded in the OnPlay method 
        // will automatically switch over to the MushuUpgradedPower!
    }
}