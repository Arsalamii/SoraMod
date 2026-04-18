using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Cards.Special;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Common;

  
[Pool(typeof(SoraModCardPool))]
public class ValorFormSoraMod() : SoraModCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
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
        get => new List<DynamicVar> { new PowerVar<ValorFormPower>(1m) };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ValorFormSoraMod card = this;

        // 1. APPLY THE VALOR FORM BUFF TO SORA
        // Even though this is a Skill card, we use PowerCmd to apply the ongoing status effect!
        await PowerCmd.Apply<ValorFormPower>(
            card.Owner.Creature,
            card.DynamicVars["ValorFormPower"].BaseValue,
            card.Owner.Creature,
            card
        );

        CardModel revertCard = ModelDb.Card<RevertSoraMod>();
        await CardPileCmd.Add(revertCard, PileType.Hand);
    }

    protected override void OnUpgrade()
    {

    }
}