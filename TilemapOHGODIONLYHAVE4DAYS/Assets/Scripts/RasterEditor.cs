using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// User interface for the tile raster.
/// </summary>
public class RasterEditor : MonoBehaviour
{
    public enum BrushMode
    {
        PAINT = 0,
        FILL = 1,
        ERASE = 2,
    }

    public static TileSwatch ERASER;
    [field: SerializeField] private TileSwatch activeBrush;
    [field: SerializeField] private ActionSet<PaintAction> currentStroke;

    [Header("References")]

    [field: SerializeField] private Sprite emptyTile;
    [field: SerializeReference] private TileRaster raster;
    [field: SerializeField] private TileSwatchBoard swatches;
    [field: SerializeField] private UnityEngine.UI.Image brushDisplay;

    public BrushMode Brush { get; set; }

    private void Start()
    {
        ERASER = new TileSwatch(0, emptyTile);
        activeBrush = new TileSwatch(0, emptyTile);
        swatches.InitPallette(ERASER);
    }

    private void Update()
    {
        brushDisplay.sprite = activeBrush.TileDisplay;
        ManageInput();
    }

    public void SetBrush(TileSwatch swatch)
    {
        activeBrush = swatch;
    }

    private void ManageInput()
    {
        bool beganStroke = false;
        if (Input.GetMouseButtonDown(0)) //Begin stroke
        {
            if (Brush == BrushMode.FILL)
            {
                ActionSet<PaintAction> fillAction = FillTiles(activeBrush);
                if (fillAction != null)
                {
                    History.PushAction(fillAction);
                }
            }
            else 
            {
                currentStroke = new ActionSet<PaintAction>();
                beganStroke = true;
            }
        }

        if ((Input.GetMouseButton(0) || beganStroke) && Brush != BrushMode.FILL)
        {
            PaintAction action = PaintTile(Brush == BrushMode.PAINT ? activeBrush : ERASER);
            if (action != null) //Only add to stroke if something changes.
            {
                currentStroke.PushAction(action);
            }
        }

        if (Input.GetMouseButtonUp(0) && Brush != BrushMode.FILL) //End stroke
        {
            if (currentStroke != null && !currentStroke.Empty())
            {
                History.PushAction(currentStroke);
                currentStroke = null;
            }
        }

        if (Input.GetMouseButtonDown(1)) //Check tile index at point
        {
            Vector2Int indexes = ScreenToTile(Input.mousePosition - new Vector3(16, 16));
            if (IsOnRaster(indexes))
            {
                print(raster.GetTileIndex(indexes.x, indexes.y));
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                History.UndoAction();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                History.RedoAction();
            }
        }
    }

    private ActionSet<PaintAction> FillTiles(TileSwatch swatch)
    {
        Vector2Int origin = ScreenToTile(Input.mousePosition - new Vector3(16, 16));
        if (IsOnRaster(origin) && raster.GetTileIndex(origin) != swatch.TileIndex)
        {
            ActionSet<PaintAction> action = new();

            int fillIndex = raster.GetTileIndex(origin.x, origin.y);
            List<Vector2Int> tilesToFill = new() { origin };

            void Evaluate(Vector2Int position, ref List<Vector2Int> tilesToAdd)
            {
                if (IsOnRaster(position) && raster.GetTileIndex(position) == fillIndex)
                {
                    tilesToAdd.Add(position);
                }
            }

            while (tilesToFill.Count > 0)
            {
                List<Vector2Int> tilesToAdd = new() {  };
                for (int t = 0; t < tilesToFill.Count; t++)
                {
                    Evaluate(tilesToFill[t] +   Vector2Int.up, ref tilesToAdd);
                    Evaluate(tilesToFill[t] +Vector2Int.right, ref tilesToAdd);
                    Evaluate(tilesToFill[t] + Vector2Int.down, ref tilesToAdd);
                    Evaluate(tilesToFill[t] + Vector2Int.left, ref tilesToAdd);
                    var paintAction = raster.SetTileSwatch(tilesToFill[t].x, tilesToFill[t].y, swatch);
                    if (paintAction != null)
                    {
                        action.PushAction(paintAction);
                    }
                }
                tilesToFill.Clear();
                for (int t = 0; t < tilesToAdd.Count; t ++)
                {
                    tilesToFill.Add(tilesToAdd[t]);
                }
            }
            return action;
        }
        return null;
    }

    private PaintAction PaintTile(TileSwatch swatch)
    {
        Vector2Int indexes = ScreenToTile(Input.mousePosition - new Vector3(16, 16));
        if (IsOnRaster(indexes))
        {
            return raster.SetTileSwatch(indexes.x, indexes.y, swatch);
        }
        return null;
    }

    /// <summary>
    /// Converts a pixel position to a tile index vector.
    /// Use in conjunction with IsOnRaster to prevent 'out of bounds' errors.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2Int ScreenToTile(Vector2 pos)
    {
        return Vector2Int.RoundToInt((pos - new Vector2(882, 45)) / 64f);
    }

    /// <summary>
    /// Returns true if a pixel position is on the tile raster.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsOnRaster(Vector2 pos)
    {
        return pos.x > 882 && pos.x < 1876 && pos.y > 45 && pos.y < 1035;
    }

    /// <summary>
    /// Returns true if an index vector is within bounds of the raster.
    /// </summary>
    /// <param name="pos">Index vector.</param>
    /// <returns></returns>
    public bool IsOnRaster(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 16 && pos.y >= 0 && pos.y < 16;
    }
}
