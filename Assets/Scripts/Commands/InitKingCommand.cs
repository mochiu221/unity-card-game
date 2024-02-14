using UnityEngine;
using System.Collections;

public class InitKingCommand : Command
{
    private Vector2Int tileIndex;
    private Player player;
    private int creatureID;

    public InitKingCommand(Player player, Vector2Int tileIndex, int creatureID)
    {
        this.player = player;
        this.tileIndex = tileIndex;
        this.creatureID = creatureID;
    }

    public override void StartCommandExecution()
    {
        CharacterAsset charAsset = player.charAsset;
        // move this card to the spot 
        ChessboardManager.Instance.AddKingAtIndex(tileIndex.x, tileIndex.y, creatureID, player);
    }
}
