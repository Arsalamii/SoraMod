using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Common;

[Pool(typeof(SoraModCardPool))]
public class StrikeRaidSoraMod() : SoraKeybladeCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // SET BASE DAMAGE TO 7
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(7m, ValueProp.Move) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        StrikeRaidSoraMod card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        
        // DEAL DAMAGE
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard((CardModel) card)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // APPLY THE DELAYED POWER
        await PowerCmd.Apply<StrikeRaidPower>(
            this.Owner.Creature, 
            1m,                  // 1 stack 
            this.Owner.Creature, // Applied by Sora, to Sora
            this
        );
    }
    
    // UPGRADE DAMAGE +3
    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3m);
}