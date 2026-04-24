using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SoraMod.SoraModCode.Powers;

public class AssistTrackerPower : PowerModel
{
    // A list of actions to run when the enemy dies
    private List<Func<Task>> _deathCallbacks;

    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.None; 
    protected override bool IsVisibleInternal => false; 

    // THE MEMORY LEAK FIX:
    // This forces the engine to give every clone its own private list!
    protected override void AfterCloned()
    {
        base.AfterCloned();
        this._deathCallbacks = new List<Func<Task>>();
    }

    // Add a packed suitcase of logic to this enemy
    public void AddCallback(Func<Task> callback)
    {
        if (callback != null)
        {
            _deathCallbacks.Add(callback);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        // 1. The Ultimate Fix: We just check if the enemy has 0 health!
        // This prevents "Cleanse" moves from granting free EXP, while perfectly
        // dodging the engine's delayed IsDead boolean.
        if (oldOwner.CurrentHp <= 0 && _deathCallbacks != null && _deathCallbacks.Count > 0)
        {
            foreach (var callback in _deathCallbacks)
            {
                await callback();
            }
        }
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.BeforeTurnEnd(choiceContext, side);

        // Optional safety: Make sure it's actually the Player's turn ending!
        if (side == CombatSide.Player) 
        {
            _deathCallbacks?.Clear(); 
            await PowerCmd.Remove(this); 
        }
    }
}