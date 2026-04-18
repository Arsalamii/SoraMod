using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Relics;

public class CrownPendant() : SoraModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get => new List<DynamicVar> { new StarsVar(3) };
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        CrownPendant crownPendant = this;
        if (!(room is CombatRoom))
            return;
        await PlayerCmd.GainStars(crownPendant.DynamicVars.Stars.BaseValue, crownPendant.Owner);
    }
}