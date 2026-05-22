using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;
using SoraMod.SoraModCode.Synergy;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class DrivesEdgeSoraMod() : SoraModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDriveFormSynergy
{
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { SoraModEnums.Keyblade };

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> 
    { 
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Draw", 1m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DrivesEdgeSoraMod card = this;
        if (cardPlay.Target == null) return;

        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // We still check if they are in the form to grant the bonus draw on play
        bool isInDriveForm = card.Owner.Creature.Powers.Any(p => p.GetType().Name.Contains("FormPower")); 

        if (isInDriveForm)
        {
            int drawAmount = (int)card.DynamicVars["Draw"].BaseValue;
            await CardPileCmd.Draw(choiceContext, drawAmount, card.Owner.Creature.Player);
        }
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3m);

    // --- THE INTERFACE METHODS ---
    // The Form Power will automatically trigger these!
    public void ApplyDriveSynergy()
    {
        this.EnergyCost.SetThisTurnOrUntilPlayed(0);
    }

    public void RemoveDriveSynergy()
    {
        this.EnergyCost.SetThisTurnOrUntilPlayed(this.EnergyCost.Canonical);
    }
}