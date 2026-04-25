using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Special;

public class ValorFormSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
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
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Exhaust
    };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>
    {
        HoverTipFactory.FromPower<ValorFormPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new PowerVar<ValorFormPower>(1m) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ValorFormSoraMod card = this;

        // APPLY THE VALOR FORM BUFF TO SORA
        await PowerCmd.Apply<ValorFormPower>(
            card.Owner.Creature,
            card.DynamicVars["ValorFormPower"].BaseValue,
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