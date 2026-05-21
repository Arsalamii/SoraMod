using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Common.Attacks;

[Pool(typeof(SoraModCardPool))]
public class SlidingDashSoraMod() : SoraKeybladeCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 1. SET BASE STATS: 3 Damage
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> 
        { 
            new DamageVar(3m, ValueProp.Move)
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SlidingDashSoraMod card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        
        // 2. DEAL DAMAGE
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard((CardModel) card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 3. CHECK IF THIS IS THE FIRST CARD PLAYED
        // We use the exact History search query from Slay the Spire 2's EchoFormPower
        int cardsPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted
            .Count(e => 
                e.Actor == this.Owner.Creature && 
                e.CardPlay.IsFirstInSeries && 
                e.HappenedThisTurn(this.Owner.Creature.CombatState)
            );

        // Since Sliding Dash is already logged in the History when OnPlay fires, 
        // if it was the first card played, the count will be exactly 1!
        if (cardsPlayedThisTurn == 1)
        {
            await CardPileCmd.Draw(choiceContext, 1, this.Owner.Creature.Player);
        }
    }
    
    // 4. UPGRADE: +2 Damage
    protected override void OnUpgrade() 
    {
        this.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}