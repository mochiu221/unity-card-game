using UnityEngine;
using System.Collections;

public class TileAsset : ScriptableObject 
{
	public string tileName;
    [TextArea(2,3)] public string description;
	public Sprite tileImage;
    public string tileScriptName;

}