using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class History : MonoBehaviour
{
    private static History instance;

    /// <summary>
    /// Returns the History instance.
    /// </summary>
    public static History Instance { get => instance; }

    [SerializeField] private Stack<Action> actions = new();
    [SerializeField] private Stack<Action> undoneActions = new();

    /// <summary>
    /// Pushes an action and clears undone actions.
    /// </summary>
    /// <param name="action"></param>
    public static void PushAction(Action action)
    {
        instance.actions.Push(action);
        instance.undoneActions.Clear();
    }
    
    /// <summary>
    /// Undoes an action if there are any to undo.
    /// </summary>
    public static void UndoAction()
    {
        instance.Undo();
    }

    /// <summary>
    /// Redoes an action if there are any to redo.
    /// </summary>
    public static void RedoAction()
    {
        instance.Redo();
    }

    /// <summary>
    /// Executes Undo code.
    /// </summary>
    private void Undo()
    {
        if (actions.Count > 0 && actions.Peek() != null)
        {
            print("Undoing");
            actions.Peek().Undo();
            undoneActions.Push(actions.Pop());
        }
    }

    /// <summary>
    /// Executes Redo code.
    /// </summary>
    private void Redo()
    {
        if (undoneActions.Count > 0 && actions.Peek() != null)
        {
            print("Redoing");
            undoneActions.Peek().Redo();
            actions.Push(undoneActions.Pop());
        }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
