using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Cards.Uncommon.Attacks;

[Pool(typeof(SoraModCardPool))]
public class SpellweaverSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS: 7 Damage, 1 Draw
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(7m, ValueProp.Move),
            new DynamicVar("Draw", 1m)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpellweaverSoraMod card = this;
        
        if (cardPlay.Target == null) return;

        // 2. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 3. DEAL DAMAGE
        // Damage happens regardless of the condition!
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 4. CHECK MAGIC CONDITION FOR THE BONUS DRAW
        bool magicPlayedThisTurn = CombatManager.Instance.History.CardPlaysFinished.Any(e => 
            e.HappenedThisTurn(this.CombatState) && 
            e.CardPlay.Card.Owner == this.Owner && 
            e.CardPlay.Card.Tags.Contains(SoraModEnums.Magic));

        if (magicPlayedThisTurn)
        {
            // 5. BONUS DRAW
            int drawAmount = (int)card.DynamicVars["Draw"].BaseValue;
            await CardPileCmd.Draw(choiceContext, drawAmount, card.Owner.Creature.Player);
        }
    }

    // 6. UPGRADE: +3 Damage (from 7 to 10)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(3m);
    }

    // --- 7. NATIVE COST REDUCTION LOGIC ---
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || this.IsClone)
            return Task.CompletedTask;

        // Check if we ALREADY played a Magic card this turn before drawing Spellweaver
        bool magicPlayedThisTurn = CombatManager.Instance.History.CardPlaysFinished.Any(e => 
            e.HappenedThisTurn(this.CombatState) && 
            e.CardPlay.Card.Owner == this.Owner && 
            e.CardPlay.Card.Tags.Contains(SoraModEnums.Magic));

        if (magicPlayedThisTurn)
        {
            // Instantly make it cost 0!
            this.EnergyCost.SetThisTurnOrUntilPlayed(0);
        }
        
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        // If Spellweaver is sitting in your hand and the card you are playing RIGHT NOW is a Magic card...
        if (cardPlay.Card.Owner == this.Owner && cardPlay.Card.Tags.Contains(SoraModEnums.Magic))
        {
            // Make Spellweaver cost 0!
            this.EnergyCost.SetThisTurnOrUntilPlayed(0);
        }

        return Task.CompletedTask;
    }
}