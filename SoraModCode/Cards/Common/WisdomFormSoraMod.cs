using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Common;

[Pool(typeof(SoraModCardPool))]
public class WisdomFormSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override bool IsPlayable
    {
        get
        {
            if (this.Owner?.PlayerCombatState != null)
            {
                return this.Owner.PlayerCombatState.Stars >= 3;
            }
            return false;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new PowerVar<WisdomFormPower>(1m) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        WisdomFormSoraMod card = this;

        // APPLY THE WISDOM FORM BUFF TO SORA
        await PowerCmd.Apply<WisdomFormPower>(
            card.Owner.Creature,
            card.DynamicVars["WisdomFormPower"].BaseValue, // Or just 1m if you aren't using a DynamicVar
            card.Owner.Creature,
            card
        );

        // CREATE THE REVERT CARD INSTANCE
        CardModel revertCard = card.CombatState.CreateCard<RevertSoraMod>(card.Owner);

        // 3. SPAWN IT INTO THE HAND
        await CardPileCmd.AddGeneratedCardsToCombat(
            new List<CardModel> { revertCard }, 
            PileType.Hand, 
            true
        );
    }

    protected override void OnUpgrade()
    {
        // Add upgrade logic if needed!
    }
}