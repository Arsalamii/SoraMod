using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))]
public class RefleraSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 10;

    public RefleraSoraMod() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(16m, ValueProp.Move) // Upgraded to 16 Block
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        // 1. GAIN BLOCK
        await CommonActions.CardBlock(this, cardPlay);

        // 2. APPLY REFLECT POWER
        await PowerCmd.Apply<SoraReflectPower>(this.Owner.Creature, 1m, this.Owner.Creature, this);

        // 3. EXP AND EVOLUTION LOGIC
        var masterDeck = PileType.Deck.GetPile(this.Owner);
        CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
            c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
        );

        if (trueMasterCard is SoraMagicCard magicMasterCard)
        {
            magicMasterCard.Experience += 1;

            if (magicMasterCard.Experience >= EvolutionRequirement)
            {
                await this.EvolveIntoReflega(magicMasterCard);
            }
        }
    }

    private async Task EvolveIntoReflega(SoraMagicCard masterDeckCard)
    {
        var newReflega = this.CardScope.CreateCard<ReflegaSoraMod>(this.Owner);
        newReflega.Experience = masterDeckCard.Experience - EvolutionRequirement;

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newReflega); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<ReflegaSoraMod>(this);
        }
    }
}