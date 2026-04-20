using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using MegaCrit.Sts2.Core.ValueProps;

namespace SoraMod.SoraModCode.Cards.Rare;

[Pool(typeof(SoraModCardPool))]
public class RagnarokSoraMod() : SoraModCard(0, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy) 
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(6m, ValueProp.Move) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Step 1: Get the exact Energy spent using the official method!
        int energySpent = this.ResolveEnergyXValue();

        // Step 2: Check current Drive (Stars)
        int currentStars = 0;
        if (this.Owner?.PlayerCombatState != null)
        {
            currentStars = (int)this.Owner.PlayerCombatState.Stars;
        }

        // Step 3: Calculate total hits!
        int totalHits = energySpent + currentStars;

        // Step 4: Fire the barrage!
        if (totalHits > 0)
        {
            for (int i = 0; i < totalHits; i++)
            {
                // We need a new random target for EVERY hit, so it scatters!
                // We use a quick C# trick to shuffle the enemies and pick the first one.
                Creature randomEnemy = this.CombatState.HittableEnemies
                    .OrderBy(e => System.Guid.NewGuid())
                    .FirstOrDefault();

                if (randomEnemy != null)
                {
                    await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                        .FromCard(this)
                        .Targeting(randomEnemy)
                        .WithHitFx("vfx/vfx_attack_magic")
                        .Execute(choiceContext);

                    await Cmd.Wait(0.1f); 
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}