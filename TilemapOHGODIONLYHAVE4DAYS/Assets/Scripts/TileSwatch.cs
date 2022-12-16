using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a tile that will be painted onto the tilemap.
/// </summary>
public class TileSwatch
{
    public int TileIndex { get; private set; }
    public Sprite TileDisplay { get; private set; }

    public TileSwatch(int index, Sprite display)
    {
        this.TileIndex = index;
        this.TileDisplay = display;
    }
}
