using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents; 
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class AerialDodgeSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public AerialDodgeSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    // 2. SET BASE STATS
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(8m, ValueProp.Move),
        new DynamicVar("Magic", 2m) 
    };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Play the Defend animation & Gain Block
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, cardPlay);

        // Safety verification pulled directly from GoForTheEyes
        System.ArgumentNullException.ThrowIfNull((object)cardPlay.Target, "cardPlay.Target");

        var monster = cardPlay.Target.Monster;

        if (monster != null && monster.NextMove != null)
        {
            // Dig into the active intents list and see if ANY of them are Buff or Debuff types!
            bool isBuffOrDebuff = monster.NextMove.Intents.Any(intent => 
                intent is BuffIntent || intent is DebuffIntent);

            if (isBuffOrDebuff)
            {
                // Fetch our draw amount cleanly
                int drawAmount = (int)this.DynamicVars["Magic"].BaseValue;
                
                // Draw cards safely using the player context
                await CardPileCmd.Draw(choiceContext, drawAmount, this.Owner.Creature.Player); 
            }
        }
    }

    // 4. THE UPGRADE
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3m);
    }
}