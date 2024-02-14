using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OneTileManager : MonoBehaviour
{
    public TileAsset tileAsset;
    public Vector2Int thisTileIndex;

    [Header("References")]
    public Transform slot;
    public Image tileImage;
    public Image tileFrameImage;
    public GameObject highlightImageObject;

    [Header("Highlight")]
    public Color highlightColor;
    public Color selectedColor;

    private bool isSelected;
    public bool IsSelected
    {
        get{ return isSelected; }
        set
        {
            isSelected = value;
            highlightImageObject.GetComponent<Image>().color = value ? selectedColor : highlightColor;
        }
    }

    private bool isHighlight;
    public bool IsHighlight
    {
        get { return isHighlight; }
        set
        {
            isHighlight = value;
            highlightImageObject.SetActive(value);
            if (IsSelected && !value)
                highlightImageObject.GetComponent<Image>().color = highlightColor;
        }
    }

    public void ReadTileFromAsset()
    {
        // Add tile image
        tileImage.sprite = tileAsset.tileImage;
    }

}