using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Common.Skills;

[Pool(typeof(SoraModCardPool))]
public class ComboPlusSoraMod() : SoraModCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 1. SET BASE STATS: Base draw is tracked via MagicVar!
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DynamicVar("Draw", 2m) 
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ComboPlusSoraMod card = this;
        
        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);

        // 3. BASE EFFECT: Draw cards based on our custom variable
        int baseDrawAmount = (int)card.DynamicVars["Draw"].BaseValue;
        await CardPileCmd.Draw(choiceContext, baseDrawAmount, card.Owner.Creature.Player);

        // 4. CHECK THE COMBO HISTORY
        var playHistory = CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Where(e => e.Actor == this.Owner.Creature)
            .ToList();

        if (playHistory.Count >= 2)
        {
            var previousPlay = playHistory[playHistory.Count - 2];
            var previousCard = previousPlay.CardPlay.Card;

            // 5. CONDITIONAL BONUS: Was it a Keyblade?
            if (previousCard != null && previousCard.Tags.Contains(SoraModEnums.Keyblade))
            {
                await CardPileCmd.Draw(choiceContext, 1, card.Owner.Creature.Player);
                card.Owner.Creature.Player.PlayerCombatState.GainEnergy(1);
            }
        }
    }

    // 6. UPGRADE: +1 Base Draw 
    protected override void OnUpgrade() 
    {
        this.DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}