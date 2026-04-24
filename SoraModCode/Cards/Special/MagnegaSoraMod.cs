using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class MagnegaSoraMod : SoraMagicCard
{
    // Rarity is Rare, and we pass 'false' at the end if you don't want it in the Main Menu library!
    public MagnegaSoraMod() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    // 1. Load BOTH debuffs into the dynamic variables list
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<VulnerablePower>(3),
        new PowerVar<WeakPower>(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Optional: Play Sora's cast animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // 2. Loop through every living enemy and apply both debuffs!
        foreach (var monster in this.CombatState.Enemies)
        {
            if (monster != null && !monster.IsDead)
            {
                // Apply 3 Vulnerable
                await PowerCmd.Apply<VulnerablePower>(
                    monster, 
                    this.DynamicVars.Vulnerable.BaseValue, 
                    this.Owner.Creature, 
                    this
                );

                // Apply 1 Weak
                await PowerCmd.Apply<WeakPower>(
                    monster, 
                    this.DynamicVars.Weak.BaseValue, 
                    this.Owner.Creature, 
                    this
                );
            }
        }
    }
}