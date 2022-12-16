using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Stores an action that can be undone or redone.
/// </summary>
[type: System.Serializable]
public abstract class Action
{
    public abstract void Undo();
    public abstract void Redo();
}
