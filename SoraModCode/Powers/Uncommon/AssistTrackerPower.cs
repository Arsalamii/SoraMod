using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SoraMod.SoraModCode.Powers.Uncommon;

public class AssistTrackerPower : PowerModel
{
    // 1. THE NULL FIX: Initialize the list immediately upon creation!
    private List<Func<Task>> _deathCallbacks = new List<Func<Task>>();

    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.None; 
    protected override bool IsVisibleInternal => false; 

    protected override void AfterCloned()
    {
        base.AfterCloned();
        // 2. THE CLONE FIX: We create a new list, but we COPY the existing callbacks into it!
        // This stops memory leaks but preserves the logic for the game's intent prediction.
        this._deathCallbacks = new List<Func<Task>>(this._deathCallbacks);
    }

    public void AddCallback(Func<Task> callback)
    {
        if (callback != null)
        {
            _deathCallbacks.Add(callback);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.CurrentHp <= 0 && _deathCallbacks != null && _deathCallbacks.Count > 0)
        {
            // 3. THE SAFE LOOP FIX: Appending .ToList() creates a temporary snapshot of the list 
            // so we don't crash if the list changes mid-execution!
            foreach (var callback in _deathCallbacks.ToList())
            {
                await callback();
            }
        }
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.BeforeTurnEnd(choiceContext, side);

        if (side == CombatSide.Player) 
        {
            _deathCallbacks?.Clear(); 
            await PowerCmd.Remove(this); 
        }
    }
}