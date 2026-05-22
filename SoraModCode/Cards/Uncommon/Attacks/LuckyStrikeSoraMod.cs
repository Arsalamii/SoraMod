using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class LuckyStrikeSoraMod() : SoraModCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 1. PREVENT INFINITE GOLD FARMING
    public override bool CanBeGeneratedInCombat => false;

    // 2. STRIKE TAG
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag> { CardTag.Strike };
    }

    // 3. EXHAUST KEYWORD
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] 
    { 
        CardKeyword.Exhaust 
    };

    // 4. ADD THE OFFICIAL 'FATAL' TOOLTIP
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get => new List<IHoverTip> { HoverTipFactory.Static(StaticHoverTip.Fatal) };
    }

    // 5. SET BASE STATS
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(5m, ValueProp.Move),
            new DynamicVar("Gold", 15m)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LuckyStrikeSoraMod card = this;
        
        // Safety check required by Slay the Spire 2 engine for targeted cards
        if (cardPlay.Target == null) return;

        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 6. CHECK IF ENEMY ALLOWS FATAL REWARDS (e.g., they aren't a respawning minion)
        bool shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());

        // 7. DEAL DAMAGE AND SAVE THE RESULT
        var attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 8. FATAL CHECK: Are they allowed to trigger fatal, AND did this specific attack kill them?
        if (shouldTriggerFatal && attackCommand.Results.Any(r => r.WasTargetKilled))
        {
            // 9. GAIN GOLD!
            int goldAmount = card.DynamicVars["Gold"].IntValue;
            await PlayerCmd.GainGold((decimal)goldAmount, card.Owner);
        }
    }

    // 10. UPGRADE: +2 Damage, +5 Gold
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(2m);
        this.DynamicVars["Gold"].UpgradeValueBy(5m);
    }
}