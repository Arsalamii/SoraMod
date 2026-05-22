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
public class VicinityBreakSoraMod() : SoraModCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    // 1. ADD KEYBLADE TAG
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag> { SoraModEnums.Keyblade };
    }

    // 2. SET BASE STATS: 12 Damage
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(12m, ValueProp.Move)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VicinityBreakSoraMod card = this;

        // 3. PLAY ANIMATION
        await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.CastAnimDelay);

        // 4. DEAL AOE DAMAGE (Using Thunderclap's TargetingAllOpponents logic!)
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard(card)
            .TargetingAllOpponents(card.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    // 5. UPGRADE: +4 Damage (from 12 to 16)
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(4m);
    }

    // --- 6. NATIVE STS2 COST REDUCTION LOGIC (Stolen from Stomp) ---
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        // Safety check native to Stomp
        if (card != this || this.IsClone)
            return Task.CompletedTask;

        // Count how many Keyblades we ALREADY played this turn
        int playedKeyblades = CombatManager.Instance.History.CardPlaysFinished.Count(e => 
            e.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade) && 
            e.CardPlay.Card.Owner == this.Owner && 
            e.HappenedThisTurn(this.CombatState));

        if (playedKeyblades > 0)
        {
            this.ReduceCostBy(playedKeyblades);
        }
        
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        // Whenever a card is played while this is in your hand, check if it's a Keyblade!
        if (cardPlay.Card.Owner != this.Owner || !cardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
            return Task.CompletedTask;

        this.ReduceCostBy(1);
        return Task.CompletedTask;
    }

    // The official method the engine uses to alter card costs mid-combat
    private void ReduceCostBy(int amount)
    {
        this.EnergyCost.AddThisTurn(-amount);
    }
}