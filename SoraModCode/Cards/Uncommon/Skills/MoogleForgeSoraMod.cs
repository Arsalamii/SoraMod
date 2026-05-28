using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class MoogleForgeSoraMod : SoraModCard
{
    // 1. CONSTRUCTOR
    public MoogleForgeSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 2. KEYWORDS (Adding Exhaust natively)
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    // 3. THE PLAY ACTION
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Trigger a Cast or Defend animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        // 1. Open the native upgrade selection screen (Stolen from Armaments!)
        CardModel selectedCard = await CardSelectCmd.FromHandForUpgrade(choiceContext, this.Owner, this);

        if (selectedCard != null)
        {
            // 2. Upgrade the card for the rest of combat (No 'await' needed!)
            CardCmd.Upgrade(selectedCard);

            // 3. Make it cost 0 this turn
            // THE COST FIX: If SetForTurn throws a red squiggle, delete it, type 'selectedCard.EnergyCost.' 
            // and look for methods like 'ModifyForTurn' or 'SetOverride'.
            selectedCard.EnergyCost.SetThisTurn(0); 
            
            // (Alternative Check: If modifying the EnergyCost object directly fails, 
            // type 'await CardCmd.' and look for a SetCostForTurn command!)
        }
    }

    // 4. THE UPGRADE (Cost 1 -> Cost 0)
    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}