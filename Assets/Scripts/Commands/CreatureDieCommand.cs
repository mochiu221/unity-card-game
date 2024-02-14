using UnityEngine;
using System.Collections;

public class CreatureDieCommand : Command 
{
    private int deadCreatureID;
    private Vector2Int tileIndex;

    public CreatureDieCommand(Vector2Int tileIndex)
    {
        this.tileIndex = tileIndex;
    }

    public override void StartCommandExecution()
    {
        ChessboardManager.Instance.RemoveCreatureByTileIndex(tileIndex);
    }
}
