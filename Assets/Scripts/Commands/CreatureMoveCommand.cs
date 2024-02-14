using UnityEngine;
using System.Collections;

public class CreatureMoveCommand : Command 
{
    private Vector2Int currentIndex;
    private Vector2Int newIndex;

    public CreatureMoveCommand(Vector2Int currentIndex, Vector2Int newIndex)
    {
        this.currentIndex = currentIndex;
        this.newIndex = newIndex;
    }

    public override void StartCommandExecution()
    {
        ChessboardManager.Instance.MoveCreature(currentIndex, newIndex);
    }
}