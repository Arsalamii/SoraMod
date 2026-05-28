using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class SinkIntoDarknessSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public SinkIntoDarknessSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. SET BASE STATS: 5 Block, 1 Drive (Stars)
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("Stars", 1m) 
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        var handPile = PileType.Hand.GetPile(this.Owner);

        // 1. Find all Status and Curse cards currently in the hand
        var cardsToExhaust = handPile.Cards
            .Where(c => c.Type == CardType.Status || c.Type == CardType.Curse)
            .ToList();

        int exhaustedCount = 0;

        // 2. Exhaust them one by one
        foreach (var card in cardsToExhaust)
        {
            // THE EXHAUST FIX: If CardCmd.Exhaust throws a squiggle, 
            // try: await CardPileCmd.Add(card, PileType.Exhaust);
            await CardCmd.Exhaust(choiceContext, card);
            
            exhaustedCount++;
        }

        // 3. If we actually exhausted anything, calculate and grant the rewards!
        if (exhaustedCount > 0)
        {
            // --- BLOCK REWARD ---
            int blockPerCard = (int)this.DynamicVars.Block.BaseValue;
            int totalBlock = blockPerCard * exhaustedCount;

// THE BLOCK FIX: The engine expects the target, the decimal amount, the ValueProp enum, and the cardPlay context!
            await CreatureCmd.GainBlock(this.Owner.Creature, (decimal)totalBlock, ValueProp.Move, cardPlay);


// --- DRIVE REWARD ---
            int starsPerCard = (int)this.DynamicVars["Stars"].BaseValue;
            int totalStars = starsPerCard * exhaustedCount;

// THE STARS FIX: this.Owner is already the Player object!
            await PlayerCmd.GainStars((decimal)totalStars, this.Owner);
        }
    }

    // 4. THE UPGRADE (5 Block -> 7 Block per card)
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2m); 
    }
}