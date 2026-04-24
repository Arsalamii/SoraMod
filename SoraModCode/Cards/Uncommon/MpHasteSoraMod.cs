using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Uncommon;

[Pool(typeof(SoraModCardPool))]
public class MpHasteSoraMod : SoraModCard
{
    // Constructor: Base Cost 1, Power Type. (Adjust CardRarity to your liking!)
    public MpHasteSoraMod() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. We completely delete the 'new MPHastePower()' line!

        // 2. We use the generic <MPHastePower> so the engine builds it for us.
        await PowerCmd.Apply<MPHastePower>(
            this.Owner.Creature, // Target (Sora)
            1m,                  // Amount (Note the 'm' to make it a decimal! StS2 requires decimals here)
            this.Owner.Creature, // Source (Sora)
            this                 // Source Card (The MP Haste card itself)
        );
    }
    
    protected override void OnUpgrade()
    {
        this.EnergyCost.SetCustomBaseCost(0); 
    }
}