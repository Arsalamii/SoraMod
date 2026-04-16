using BaseLib.Abstracts;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;

namespace SoraMod.SoraModCode.Potions;

[Pool(typeof(SoraModPotionPool))]
public abstract class SoraModPotion : CustomPotionModel;