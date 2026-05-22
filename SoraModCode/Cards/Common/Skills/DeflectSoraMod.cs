using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class DeflectSoraMod() : SoraModCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag> { CardTag.Defend };
    }

    public override bool GainsBlock => true;

    // 1. SET BASE STATS: 5 Base Block, and a custom variable for the 3 Bonus Block
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new BlockVar(5m, ValueProp.Move),
            new DynamicVar("Bonus", 3m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DeflectSoraMod card = this;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 3. APPLY BASE BLOCK
        // Uses the standard block engine mechanic
        await CommonActions.CardBlock(card, cardPlay);

        // 4. CHECK FOR KEYBLADES IN HAND
        // Using the exact Creature.Player routing we locked in!
        var hand = card.Owner.Creature.Player.Piles.FirstOrDefault(p => p.Type == PileType.Hand);
        
        if (hand != null)
        {
            // Scan the hand to see if any card has the Keyblade tag
            bool hasKeyblade = hand.Cards.Any(c => c.Tags.Contains(SoraModEnums.Keyblade));

            if (hasKeyblade)
            {
                // 5. APPLY BONUS BLOCK
                var bonusBlockVar = new BlockVar(card.DynamicVars["Bonus"].BaseValue, ValueProp.Move);
                await CreatureCmd.GainBlock(card.Owner.Creature, bonusBlockVar, cardPlay);
            }
        }
    }

    // 6. UPGRADE: +3 Base Block
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Block.UpgradeValueBy(3m);
        // We leave the Bonus variable untouched so it stays at exactly 3!
    }
}