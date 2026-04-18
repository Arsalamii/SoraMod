using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Extensions;

namespace SoraMod.SoraModCode.Relics;

[Pool(typeof(SoraModRelicPool))]
public abstract class SoraModRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}