using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushButton : MonoBehaviour
{
    [SerializeField] private RasterEditor.BrushMode brushMode;

    [SerializeField] private RasterEditor raster;
    [SerializeField] private Button button;

    public void Select()
    {
        raster.Brush = brushMode;
        button.interactable = false;
    }

    private void FixedUpdate()
    {
        button.interactable = brushMode != raster.Brush;
    }
}
