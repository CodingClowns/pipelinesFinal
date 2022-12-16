using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores a tile sprite change.
/// </summary>
[type: System.Serializable]
public class PaintAction : Action
{
    [field: SerializeField] private Image targetImage;
    [field: SerializeField] private IntWrapper targetIndex;
    [field: SerializeField] private TileSwatch beforeSwatch;
    [field: SerializeField] private TileSwatch afterSwatch;

    public PaintAction(ref Image targetImage, ref IntWrapper targetIndex, TileSwatch beforeSwatch, TileSwatch afterSwatch)
    {
        this.targetIndex = targetIndex;
        this.targetImage = targetImage;
        this.beforeSwatch = beforeSwatch;
        this.afterSwatch = afterSwatch;
    }

    /// <summary>
    /// Undoes the action.
    /// </summary>
    public override void Undo()
    {
        targetImage.sprite = beforeSwatch.TileDisplay;
        targetIndex.value = beforeSwatch.TileIndex;
    }

    /// <summary>
    /// Redoes the action.
    /// </summary>
    public override void Redo()
    {
        targetImage.sprite = afterSwatch.TileDisplay;
        targetIndex.value = afterSwatch.TileIndex;
    }
}
