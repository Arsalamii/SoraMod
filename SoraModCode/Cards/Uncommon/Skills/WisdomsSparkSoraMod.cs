using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;
using SoraMod.SoraModCode.Powers.Forms; 

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class WisdomsSparkSoraMod : SoraMagicCard
{
    public WisdomsSparkSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<VulnerablePower>(1),
        new PowerVar<WeakPower>(1),
        new DynamicVar("Combo", 3m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // 1. CHECK THE COMBO CONDITION
        var lastCardEntry = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(e => 
                e.HappenedThisTurn(this.CombatState) && 
                e.CardPlay.Card.Owner == this.Owner);

        bool isComboActive = lastCardEntry != null && 
                             lastCardEntry.CardPlay.Card.Tags.Contains(SoraModEnums.Magic);

        // 2. DETERMINE BASE DEBUFF AMOUNTS
        decimal vulnToApply = isComboActive ? this.DynamicVars["Combo"].BaseValue : this.DynamicVars.Vulnerable.BaseValue;
        decimal weakToApply = isComboActive ? this.DynamicVars["Combo"].BaseValue : this.DynamicVars.Weak.BaseValue;

        // --- 3. WISDOM FORM CHECK ---
        if (this.Owner.HasPower<WisdomFormPower>())
        {
            // Add +1 to both debuffs if Wisdom Form is active! (Adjust this number as needed)
            vulnToApply += 2;
            weakToApply += 2;
        }

        // 4. APPLY DEBUFFS TO ALL ENEMIES
        foreach (var monster in this.CombatState.Enemies)
        {
            if (monster != null && !monster.IsDead)
            {
                await PowerCmd.Apply<VulnerablePower>(
                    monster, 
                    vulnToApply, 
                    this.Owner.Creature, 
                    this
                );

                await PowerCmd.Apply<WeakPower>(
                    monster, 
                    weakToApply, 
                    this.Owner.Creature, 
                    this
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Vulnerable.UpgradeValueBy(1m);
        this.DynamicVars.Weak.UpgradeValueBy(1m);
        this.DynamicVars["Combo"].UpgradeValueBy(1m);
    }
}