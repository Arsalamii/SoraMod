using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Forms;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class MagnegaSoraMod : SoraMagicCard
{
    public MagnegaSoraMod() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<VulnerablePower>(3),
        new PowerVar<WeakPower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // --- WISDOM FORM CHECK ---
        decimal finalVulnerable = this.DynamicVars.Vulnerable.BaseValue;
        decimal finalWeak = this.DynamicVars.Weak.BaseValue;
        if (this.Owner.HasPower<WisdomFormPower>())
        {
            finalVulnerable += 2;
            finalWeak += 2;
        }

        foreach (var monster in this.CombatState.Enemies)
        {
            if (monster != null && !monster.IsDead)
            {
                // Apply final dynamically calculated Vulnerable
                await PowerCmd.Apply<VulnerablePower>(
                    monster, 
                    finalVulnerable, 
                    this.Owner.Creature, 
                    this
                );

                // Apply Weak (Unchanged by Wisdom Form)
                await PowerCmd.Apply<WeakPower>(
                    monster, 
                    finalWeak, 
                    this.Owner.Creature, 
                    this
                );
            }
        }
    }
    
    public override int MaxUpgradeLevel => 0;
}