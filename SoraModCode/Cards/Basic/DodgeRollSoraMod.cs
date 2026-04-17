using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;

namespace SoraMod.SoraModCode.Cards.Basic;

public class DodgeRollSoraMod() : SoraModCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{

    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>() { CardTag.Defend };
    }

    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new BlockVar(4m, ValueProp.Move) };
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DodgeRollSoraMod cardSource = this;
        Decimal amount = await CreatureCmd.GainBlock(cardSource.Owner.Creature, cardSource.DynamicVars.Block, cardPlay);
        BlockNextTurnPower blockNextTurnPower = await PowerCmd.Apply<BlockNextTurnPower>(cardSource.Owner.Creature, amount, cardSource.Owner.Creature, (CardModel) cardSource);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(2M);
}