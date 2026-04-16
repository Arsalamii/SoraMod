using BaseLib.Abstracts;
using SoraMod.SoraModCode.Extensions;
using Godot;

namespace SoraMod.SoraModCode.Character;

public class SoraModPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => SoraMod.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}