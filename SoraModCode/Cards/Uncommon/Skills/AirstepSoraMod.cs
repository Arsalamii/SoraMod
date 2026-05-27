using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class AirstepSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR (Cost 0, Target Self)
    public AirstepSoraMod() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. SET BASE STATS: 2 Magic (Card Draw)
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DynamicVar("Magic", 2m)
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Play the Cast animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // Fetch our draw amount dynamically from the base value
        int drawAmount = (int)this.DynamicVars["Magic"].BaseValue;

        // Draw the cards using your verified Dodge Roll syntax
        await CardPileCmd.Draw(choiceContext, drawAmount, this.Owner.Creature.Player);

        // Apply the restriction power to Sora
        await PowerCmd.Apply<AirstepRestrictionPower>(
            this.Owner.Creature, 
            1m, 
            this.Owner.Creature, 
            this
        );
    }

    // 4. THE UPGRADE: Draw 3 cards instead of 2
    protected override void OnUpgrade()
    {
        this.DynamicVars["Magic"].UpgradeValueBy(1m);
    }
}