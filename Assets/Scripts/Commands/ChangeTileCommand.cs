using UnityEngine;
using System.Collections;

public class ChangeTileCommand : Command 
{
    private Vector2Int tileIndex;
    private TileAsset tileAsset;

    public ChangeTileCommand(Vector2Int tileIndex, TileAsset tileAsset)
    {
        this.tileIndex = tileIndex;
        this.tileAsset = tileAsset;
    }

    public override void StartCommandExecution()
    {
        ChessboardManager.Instance.ChangeTileByIndex(tileIndex, tileAsset);
    }
}