using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers; 
using SoraMod.SoraModCode.DynamicVars;
using BaseLib.Extensions;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))] 
public class AeroraSoraMod : SoraMagicCard
{
    private const int EvolutionRequirement = 3;

    public AeroraSoraMod() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 1. Use our custom UI variable!
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new SoraPlatingVar(6)
    };

    // 2. Manually tell the game to show the Plating tooltip when the player hovers over Aero!
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>
    {
        HoverTipFactory.FromPower<PlatingPower>()
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Buff", this.Owner.Character.CastAnimDelay);

        // 3. We bypass the engine error by just doing the math securely right here!
        decimal finalPlating = 6; // Base value
        if (this.Owner.HasPower<WisdomFormPower>())
        {
            finalPlating += 4; // Wisdom form bonus
        }

        // Apply the calculated Plating
        await PowerCmd.Apply<PlatingPower>(
            this.Owner.Creature, 
            finalPlating, 
            this.Owner.Creature, 
            this
        );

        // 4. The Bulletproof EXP and Evolution logic!
        var masterDeck = PileType.Deck.GetPile(this.Owner);
        CardModel trueMasterCard = this.DeckVersion ?? masterDeck?.Cards.FirstOrDefault(c => 
            c is SoraMagicCard smc && smc.MagicSerialNumber == this.MagicSerialNumber
        );

        if (trueMasterCard is SoraMagicCard magicMasterCard)
        {
            magicMasterCard.Experience += 1;

            if (magicMasterCard.Experience >= EvolutionRequirement)
            {
                await this.EvolveIntoAeroga(magicMasterCard);
            }
        }
    }

    // ... (Keep your standard EvolveIntoAerora logic down here) ...
    public async Task EvolveIntoAeroga(SoraMagicCard masterDeckCard)
    {
        var newAerora = this.CardScope.CreateCard<AerogaSoraMod>(this.Owner);
        if (this.IsUpgraded)
        {
            newAerora.UpgradeInternal();
            newAerora.FinalizeUpgradeInternal();
        }

        if (masterDeckCard != null)
        {
            var masterDeck = PileType.Deck.GetPile(this.Owner);
            if (masterDeck != null && masterDeck.Cards.Contains(masterDeckCard))
            {
                masterDeckCard.RemoveFromCurrentPile(); 
                masterDeck.AddInternal(newAerora); 
            }
        }

        if (!MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding)
        {
            await CardCmd.TransformTo<AeroraSoraMod>(this);
        }
    }
}