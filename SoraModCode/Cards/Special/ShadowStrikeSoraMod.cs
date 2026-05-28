using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class ShadowStrikeSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR (0 Cost, Attack, Token Rarity)
    public ShadowStrikeSoraMod() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
    }

    // 2. KEYWORDS & TAGS (Exhaust and Strike)
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };

    // 3. BASE STATS: 6 Damage
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(6m, ValueProp.Move)
    };

    // 4. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull((object)cardPlay.Target, "cardPlay.Target");
        
        // Attack animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Attack", this.Owner.Character.CastAnimDelay);

        // Deal the damage
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    // 5. THE UPGRADE (Deals 9 damage instead of 6)
    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}