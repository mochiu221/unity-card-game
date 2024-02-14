using UnityEngine;
using System.Collections;

public class StartATurnCommand : Command {

    private Player player;

    public StartATurnCommand(Player player)
    {
        this.player = player;
    }

    public override void StartCommandExecution()
    {
        if (UIManager.Instance.SelectingTileCardDA != null)
        {
            CardDraggingActions da = UIManager.Instance.SelectingTileCardDA;
            da.tilesList.Clear();
            da.BackToHand();
            UIManager.Instance.SelectingTileCardDA = null;
        }
        
        TurnManager.Instance.whoseTurn = player;
        TurnManager.Instance.turnCount ++;
        // this command is completed instantly
        CommandExecutionComplete();
    }
}