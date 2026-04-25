using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.DynamicVars;
using BaseLib.Extensions;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards.Special;

[Pool(typeof(SoraEvolutionPool))] 
public class AerogaSoraMod : SoraMagicCard
{
    public AerogaSoraMod() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    // 1. Use the custom UI variable set to 8!
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new SoraPlatingVar(10)
    };

    // 2. Ensure the tooltip still shows up
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>
    {
        HoverTipFactory.FromPower<PlatingPower>()
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Play Sora's buff animation
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Buff", this.Owner.Character.CastAnimDelay);

        // 3. Calculate the final math (Base 8 + Wisdom Form Bonus)
        decimal finalPlating = 10; 
        if (this.Owner.HasPower<WisdomFormPower>())
        {
            finalPlating += 4; 
        }

        // 4. Apply the Plated Armor to Sora!
        await PowerCmd.Apply<PlatingPower>(
            this.Owner.Creature, 
            finalPlating, 
            this.Owner.Creature, 
            this
        );
    }
}