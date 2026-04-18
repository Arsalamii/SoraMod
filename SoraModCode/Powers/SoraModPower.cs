using BaseLib.Abstracts;
using BaseLib.Extensions;
using SoraMod.SoraModCode.Extensions;

namespace SoraMod.SoraModCode.Powers;

public abstract class SoraModPower : CustomPowerModel
{
    //Loads from SoraMod/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}