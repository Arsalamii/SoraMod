using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Rare;

[Pool(typeof(SoraModCardPool))]
public class ReactionCommandSoraMod() : SoraKeybladeCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new BlockVar(15m, ValueProp.Move) };
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ReactionCommandSoraMod card = this;
        
        await CreatureCmd.GainBlock(card.Owner.Creature, card.DynamicVars.Block, cardPlay);
        
        await PowerCmd.Apply<ReactionCommandPower>(
            card.Owner.Creature, 
            1m, 
            card.Owner.Creature, 
            card
        );
    }
    
    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1); 
    }
}