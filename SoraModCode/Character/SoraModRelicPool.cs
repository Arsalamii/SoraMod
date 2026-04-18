using BaseLib.Abstracts;
using Godot;
using SoraMod.SoraModCode.Extensions;

namespace SoraMod.SoraModCode.Character;

public class SoraModRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => SoraMod.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}