using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers.Uncommon;

namespace SoraMod.SoraModCode.Cards.Uncommon.Skills;

[Pool(typeof(SoraModCardPool))]
public class ReflectSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 5;

    public ReflectSoraMod() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new BlockVar(12m, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Defend", this.Owner.Character.CastAnimDelay);

        // 1. GAIN BLOCK (Using the native helper!)
        await CommonActions.CardBlock(this, cardPlay);

        // 2. APPLY REFLECT POWER
        await PowerCmd.Apply<ReflectPower>(this.Owner.Creature, 1m, this.Owner.Creature, this);

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
                await this.EvolveIntoReflera(magicMasterCard);
            }
        }
    }

    private async Task EvolveIntoReflera(SoraMagicCard masterDeckCard)
    {
        var newReflera = this.CardScope.CreateCard<RefleraSoraMod>(this.Owner);
        
        // Subtract the 10 required EXP so it cleanly carries over any extra!
        newReflera.Experience = masterDeckCard.Experience - EvolutionRequirement;

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newReflera); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<RefleraSoraMod>(this);
        }
    }
}