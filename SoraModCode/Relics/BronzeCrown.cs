using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using SoraMod.SoraModCode.Cards.Special;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SoraMod.SoraModCode.Relics;

public sealed class BronzeCrown() : SoraModRelic
{
    private int _elitesDefeated;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => this.ElitesDefeated;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DynamicVar("Elites", 5m) };
    }

    [SavedProperty]
    public int ElitesDefeated
    {
        get => this._elitesDefeated;
        set
        {
            this.AssertMutable();
            this._elitesDefeated = value;
            this.InvokeDisplayAmountChanged();
        }
    }

    // START OF COMBAT & TURN LOGIC
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == this.Owner.Creature.Side)
        {
            // Generate the Drive Menu and Grant 3 Stars at the start of combat
            if (combatState.RoundNumber == 1)
            {
                this.Flash();
                this.Owner.PlayerCombatState.GainStars(3m); 
                // Forge the Drive card using the combatState the engine handed us
                var driveCard = combatState.CreateCard<DriveFormsSoraMod>(this.Owner);
                
                // Drop it straight into the player's hand
                await CardPileCmd.AddGeneratedCardsToCombat(new List<CardModel> { driveCard }, PileType.Hand, true);
            }
        }
        
        await base.AfterSideTurnStart(side, combatState);
    }

    // 3. END OF COMBAT: Track Elites and Evolve!
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType != RoomType.Elite)
            return;
            
        this.ElitesDefeated++;
        this.Flash();
        
        if (this.ElitesDefeated >= this.DynamicVars["Elites"].BaseValue)
        {
            // Replace this relic with the Silver Crown!
            await RelicCmd.Replace(this, ModelDb.Relic<SilverCrown>().ToMutable());
        }
    }
}