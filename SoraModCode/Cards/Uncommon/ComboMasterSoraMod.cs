using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Powers;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Cards.Uncommon;

[Pool(typeof(SoraModCardPool))]
public class ComboMasterSoraMod : SoraModCard 
{
    // Base Cost 1, Power Type, Uncommon Rarity.
    public ComboMasterSoraMod() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Apply our new Combo Master Power!
        await PowerCmd.Apply<ComboMasterPower>(
            this.Owner.Creature,
            1m,                  // 1 stack means refunding 1 Energy per trigger
            this.Owner.Creature,
            this
        );
    }
    
    // The Upgrade Logic: Add the Innate keyword!
    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}