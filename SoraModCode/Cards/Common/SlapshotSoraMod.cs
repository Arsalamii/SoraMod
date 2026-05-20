using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Enums;

namespace SoraMod.SoraModCode.Cards.Common;

[Pool(typeof(SoraModCardPool))]
public class SlapshotSoraMod() : SoraKeybladeCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // SET BASE DAMAGE TO 4
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new DamageVar(4m, ValueProp.Move) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SlapshotSoraMod card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        
        // DEAL DAMAGE
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .FromCard((CardModel) card)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // CHECK COMBAT HISTORY
        var previousCardEntry = CombatManager.Instance.History.Entries
            .OfType<CardPlayStartedEntry>()
            // We use LastOrDefault, strictly filtering out THIS exact card so it doesn't count itself!
            .LastOrDefault(e => e.CardPlay.Card != this); 

        // TRIGGER COMBO DRAW
        if (previousCardEntry != null && previousCardEntry.CardPlay.Card.Tags.Contains(SoraModEnums.Keyblade))
        {
            await CardPileCmd.Draw(choiceContext, 1, this.Owner.Creature.Player);
        }
    }
    
    // UPGRADE DAMAGE +2
    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(2m);
}