using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Powers;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon;

[Pool(typeof(SoraModCardPool))]
public class KeybladeMasterySoraMod : SoraModCard // (Or CustomCardModel)
{
    // Base Cost 1, Power Type.
    public KeybladeMasterySoraMod() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // We use the exact same generic <T> structure from Valor Form!
        await PowerCmd.Apply<KeybladeMasteryPower>(
            this.Owner.Creature,
            1m,                  // Base amount of Dexterity granted per trigger
            this.Owner.Creature,
            this
        );
    }
    
    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}