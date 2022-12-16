using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Action that carries multiple smaller actions, allowing them to be undone/redone in one input.
/// </summary>
/// <typeparam name="T">The type of action to hold.</typeparam>
[Serializable]
public class ActionSet<T> : Action where T : Action
{
    [field: SerializeField] protected Stack<T> actions = new();
    [field: SerializeField] protected Stack<T> undoneActions = new();

    /// <summary>
    /// Returns true if this set is empty.
    /// </summary>
    /// <returns></returns>
    public bool Empty()
    {
        return actions.Count == 0 && undoneActions.Count == 0;
    }

    /// <summary>
    /// Adds an action to this set.
    /// </summary>
    /// <param name="action"></param>
    public void PushAction(T action)
    {
        actions.Push(action);
    }

    /// <summary>
    /// Redoes this entire set.
    /// </summary>
    public override void Redo()
    {
        while (undoneActions.Count > 0)
        {
            undoneActions.Peek().Redo();
            actions.Push(undoneActions.Pop());
        }
    }

    /// <summary>
    /// Undoes this entire set.
    /// </summary>
    public override void Undo()
    {
        while (actions.Count > 0)
        {
            actions.Peek().Undo();
            undoneActions.Push(actions.Pop());
        }
    }
}
