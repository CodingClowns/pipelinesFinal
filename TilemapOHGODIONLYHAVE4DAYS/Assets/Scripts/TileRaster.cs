using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that can access individual tiles.
/// </summary>
public class TileRaster : MonoBehaviour
{
    [SerializeField] private string savePath;
    [SerializeField] private Tilemap activeTilemap = new();
    [SerializeField] private TileSwatchBoard swatches;

    /// <summary>
    /// Returns the tile at the specified coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GameObject GetTile(int x, int y)
    {
        return transform.GetChild(x).GetChild(15 - y).gameObject;
    }

    /// <summary>
    /// Returns a component of the requested tile.
    /// </summary>
    /// <typeparam name="T">Component to get.</typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T GetTile<T>(int x, int y) where T : Component
    {
        //print(x + ", " + y);
        return transform.GetChild(x).GetChild(15 - y).GetComponent<T>();
    }

    public int GetTileIndex(int x, int y)
    {
        return activeTilemap.tile[x][y].value;
    }

    public int GetTileIndex(Vector2Int indexes)
    {
        return activeTilemap.tile[indexes.x][indexes.y].value;
    }

    /// <summary>
    /// Replaces a tile at a position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="tile"></param>
    public void SetTile(int x, int y, GameObject tile)
    {
        GameObject oldTile = GetTile(x, y);
        tile.transform.SetParent(oldTile.transform.parent);
        tile.transform.SetSiblingIndex(oldTile.transform.GetSiblingIndex());
        Destroy(oldTile);
    }

    /// <summary>
    /// Changes the sprite of a tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sprite"></param>
    public PaintAction SetTileSwatch(int x, int y, TileSwatch swatch)
    {
        int index = GetTileIndex(x, y);
        if (index != swatch.TileIndex)
        {
            Image image = GetTile<Image>(x, y);
            TileSwatch old = new(index, image.sprite);
            image.sprite = swatch.TileDisplay;
            activeTilemap.tile[x][y].value = swatch.TileIndex;

            return new PaintAction(ref image, ref activeTilemap.tile[x][y], old, swatch);
        }
        return null;
    }

    public void LoadTilemap()
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel("Import Tilemap", "C:/", "xml");
        if (!string.IsNullOrEmpty(path))
        {
            var newTiles = Tilemap.Deserialize(path);

            if (newTiles == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("Error!", "This is not a valid XML file.", "Ok");
                return;
            }

            savePath = path;
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    var index = swatches.GetSwatchIndex(newTiles.tile[x][y].value);
                    SetTileSwatch(x, y, new TileSwatch(newTiles.tile[x][y].value, swatches.GetSwatch<Image>(index.x, index.y).sprite));
                }
            }
        }
    }

    public void SaveTilemap()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            savePath = SaveTileMapAs("C:\\");
        }

        if (!string.IsNullOrEmpty(savePath))
        {
            activeTilemap.Serialize(savePath);
        }
    }

    public string SaveTileMapAs(string path)
    {
        return UnityEditor.EditorUtility.SaveFilePanel("Save Tilemap As...", path, "tilemap.xml", "xml");
    }

    public void SaveAs()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            SaveTileMapAs("C:\\");
        }
        else
        {
            var split = savePath.LastIndexOf('\\');
            savePath = SaveTileMapAs(savePath.Remove(split, savePath.Length - split - 1));
        }
        if (!string.IsNullOrEmpty(savePath))
        {
            SaveTilemap();
        }
    }
}
