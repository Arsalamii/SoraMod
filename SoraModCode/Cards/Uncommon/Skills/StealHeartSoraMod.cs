using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Cards.Special;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class StealHeartSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public StealHeartSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        var handPile = PileType.Hand.GetPile(this.Owner);

        // Find all Status and Curse cards in the hand
        var cardsToTransform = handPile.Cards
            .Where(c => c.Type == CardType.Status || c.Type == CardType.Curse)
            .ToList();

        foreach (var card in cardsToTransform)
        {
            // 1. Create the new Shadow Strike (Using your exact evolution system syntax!)
            var shadowStrike = this.CombatState.CreateCard<ShadowStrikeSoraMod>(this.Owner);
            
            // If Steal Heart is upgraded, upgrade the generated Shadow Strikes too!
            if (this.IsUpgraded)
            {
                shadowStrike.UpgradeInternal();
                shadowStrike.FinalizeUpgradeInternal();
            }

            // 2. Safe "Transformation" (Exhaust the old, Add the new)
            await CardCmd.Exhaust(choiceContext, card);
            await CardPileCmd.Add(shadowStrike, PileType.Hand);
            
            // NOTE: If your IDE has a direct transformation command like this:
            // await CardCmd.Transform(choiceContext, card, shadowStrike);
            // You can use that instead of Exhaust/Add, but the Exhaust method is 100% bulletproof!
        }
    }

    // 3. THE UPGRADE
    protected override void OnUpgrade()
    {
        // We don't need to change any numbers here! 
        // Upgrading Steal Heart simply makes `this.IsUpgraded` return true in the OnPlay method above,
        // which automatically generates upgraded Shadow Strikes.
    }
}