using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TileSwatchBoard : MonoBehaviour
{
    [SerializeField] private List<string> errors = new();
    [SerializeField] private TilePallette activePallette = new();
    [SerializeField] private RasterEditor raster;
    [SerializeField] private string savePath;
    [SerializeField] private Toggle removingTiles;

    public void InitPallette(TileSwatch swatch)
    {
        activePallette = new();
        activePallette.paths[0] = "";
        activePallette.Count++;
        SetSwatchSprite(0, 0, swatch.TileDisplay);
    }

    /// <summary>
    /// Returns the swatch at the specified coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GameObject GetSwatch(int x, int y)
    {
        return transform.GetChild(x).GetChild(y).gameObject;
    }

    /// <summary>
    /// Returns a component of the requested swatch.
    /// </summary>
    /// <typeparam name="T">Component to get.</typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T GetSwatch<T>(int x, int y) where T : Component
    {
        return transform.GetChild(x).GetChild(y).GetComponent<T>();
    }

    public int GetSwatchIndex(int x, int y)
    {
        return (x % 8) * 8 + (y % 8);
    }

    public Vector2Int GetSwatchIndex(int index)
    {
        return new Vector2Int(index / 8, index % 8);
    }

    /// <summary>
    /// Replaces a swatch at a position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="tile"></param>
    public void SetSwatch(int x, int y, GameObject swatch)
    {
        GameObject oldSwatch = GetSwatch(x, y);
        swatch.transform.SetParent(oldSwatch.transform.parent);
        swatch.transform.SetSiblingIndex(oldSwatch.transform.GetSiblingIndex());
        Destroy(oldSwatch);
    }

    /// <summary>
    /// Changes the sprite of a swatch.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sprite"></param>
    public void SetSwatchSprite(int x, int y, Sprite sprite)
    {
        GetSwatch<Image>(x, y).sprite = sprite;
    }

    public void LoadTilePallette()
    {
        errors.Clear();
        string path = UnityEditor.EditorUtility.OpenFilePanel("Import Tile Pallette", "C:/", "xml");
        if (!string.IsNullOrEmpty(path)) 
        { 
            var newPallette = TilePallette.Deserialize(path);
            if (newPallette == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("Error!", "This is not a valid XML file.", "Ok");
                return;
            }

            activePallette = newPallette;
            savePath = path;
        }
        UpdateSwatches();
    }

    public void SaveTilePallette()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            savePath = SaveTilePalletteAs("C:/");
        }
        if (string.IsNullOrEmpty(savePath))
        {
            return;
        }

        activePallette.Serialize(savePath);
    }

    public string SaveTilePalletteAs(string path)
    {
        if (errors.Count == 0)
        {
            return UnityEditor.EditorUtility.SaveFilePanel("Save Tile Pallette As...", path, "tilepallette.xml", "xml");
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("Error!", errors[0], "Ok");
            return null;
        }
    }

    public void SaveAs()
    {
        if (!string.IsNullOrEmpty(savePath))
        {
            var split = savePath.LastIndexOf('\\');
            savePath = SaveTilePalletteAs(savePath.Remove(split, savePath.Length - split - 1));
        }
        else
        {
            savePath = SaveTilePalletteAs("C:\\");
        }
        if (!string.IsNullOrEmpty(savePath))
        {
            SaveTilePallette();
        }
    }

    private void Update()
    {
        ManageInput();
    }

    public void UpdateSwatches()
    {
        for (int s = 1; s < 64; s++)
        {
            if (!string.IsNullOrEmpty(activePallette.paths[s]))
            {
                if (File.Exists(activePallette.paths[s]))
                {
                    Texture2D texture = new(2, 2);

                    if (texture.LoadImage(File.ReadAllBytes(activePallette.paths[s])))
                    {
                        texture.filterMode = FilterMode.Point;
                        Vector2Int indexes = GetSwatchIndex(s);
                        SetSwatchSprite(indexes.x, indexes.y, Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one * 0.5f));
                    }
                    else
                    {
                        errors.Add("Could not load image " + activePallette.paths[s]);
                    }
                }
                else
                {
                    errors.Add("File " + activePallette.paths[s] + " does not exist.");
                }
            }
            else
            {
                Vector2Int indexes = GetSwatchIndex(s);
                if (GetSwatch<Image>(indexes.x, indexes.y).sprite != null) {
                    SetSwatchSprite(indexes.x, indexes.y, null);
                }
            }
        }
    }

    public void AddTile()
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel("Add Tile...", (string.IsNullOrEmpty(savePath)) ? "C:\\" : savePath, "png");
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            if (activePallette.Count >= 64)
            {
                print("Too many tiles in pallette.");
            }
            else
            {
                activePallette.paths[activePallette.Count] = path;
                activePallette.Count++;
                UpdateSwatches();
            }
        }
    }

    public void RemoveTile(int index)
    {
        if (!string.IsNullOrEmpty(activePallette.paths[index]))
        {
            for (int t = index; t < activePallette.Count - 1; t++)
            {
                activePallette.paths[t] = activePallette.paths[t + 1];
            }
            activePallette.paths[activePallette.Count - 1] = "";
            UpdateSwatches();
            print("Removed tile!!!!!!");
        }
    }

    private void ManageInput()
    {
        if (Input.GetMouseButtonDown(0)) //Try select a swatch
        {
            Vector2Int indexes = ScreenToSwatch(Input.mousePosition - new Vector3(16, 16));
            if (IsOnPallette(indexes))
            {
                if (removingTiles.isOn)
                {
                    RemoveTile(GetSwatchIndex(indexes.y, indexes.x));
                }
                else
                {
                    raster.SetBrush(new TileSwatch(GetSwatchIndex(indexes.y, indexes.x), GetSwatch<Image>(indexes.y, indexes.x).sprite));
                }
                
            }
            //print(indexes);
        }
    }

    public Vector2Int ScreenToSwatch(Vector2 pos)
    {
        Vector2Int inV = Vector2Int.RoundToInt((pos - new Vector2(44, 44)) / 64f);
        return new Vector2Int(inV.x, 7 - inV.y);
    }

    public bool IsOnPallette(Vector2 pos)
    {
        return pos.x > 44 && pos.x < 540 && pos.y > 44 && pos.y < 540;
    }

    public bool IsOnPallette(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }
}
