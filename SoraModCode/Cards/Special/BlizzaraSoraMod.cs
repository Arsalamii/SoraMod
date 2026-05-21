using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class BlizzaraSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public BlizzaraSoraMod() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new DamageVar(10m, ValueProp.Move),
        new PowerVar<WeakPower>(2m)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var targetCreature = cardPlay.Target.Monster?.Creature;
        int hpBefore = targetCreature?.CurrentHp ?? 0;

        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var weakAmount = this.DynamicVars.Select(v => v.Value).OfType<PowerVar<WeakPower>>().First().BaseValue;
    
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target, 
            weakAmount, 
            this.Owner.Creature, 
            this
        );

        // FATAL CHECK
        if (targetCreature != null && (targetCreature.IsDead || (hpBefore > 0 && targetCreature.CurrentHp <= 0)))
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            
            CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
                c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
            );

            if (trueMasterCard is SoraMagicCard magicMasterCard)
            {
                magicMasterCard.Experience += 1;

                if (magicMasterCard.Experience >= EvolutionRequirement)
                {
                    await this.EvolveIntoBlizzaga(magicMasterCard);
                }
            }
        }
    }

    private async Task EvolveIntoBlizzaga(SoraMagicCard masterDeckCard)
    {
        var newBlizzaga = this.CardScope.CreateCard<BlizzagaSoraMod>(this.Owner);
        if (this.IsUpgraded)
        {
            newBlizzaga.UpgradeInternal();
            newBlizzaga.FinalizeUpgradeInternal();
        }

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newBlizzaga); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<BlizzagaSoraMod>(this);
        }
    }
    
    public override int MaxUpgradeLevel => 0;
}