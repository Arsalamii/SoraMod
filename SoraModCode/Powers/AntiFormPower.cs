using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SoraMod.SoraModCode.Powers;

// 1. We attach the IHealAmountModifier interface to our power
  
public class AntiFormPower : SoraModPower, IHealAmountModifier
{
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Single;

    // 2. DOUBLE DAMAGE ENFORCEMENT (From AbstractModel)
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner)
        {
            return 2m; // Deal Double Damage!
        }
        return 1m; 
    }

    // 3. CANNOT HEAL ENFORCEMENT (From your BaseLib discovery!)
    public Decimal ModifyHealMultiplicative(Creature creature, Decimal amount)
    {
        if (creature == this.Owner)
        {
            return 0m; // Multiply any incoming heal by 0
        }
        return 1m; // Normal healing for everyone else
    }
}