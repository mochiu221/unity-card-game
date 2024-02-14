using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerTurnMaker : TurnMaker 
{
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        string message;
        
        if (GameModeManager.Instance.GetGameMode() == GameMode.Multiplayer)
        {
            if (player == PlayersManager.Instance.myPlayer)
            {
                message = "輪到你了!";
            }
            else
            {
                message = "輪到敵人!";
            }
        }
        else
        {
            if (player.Team == PlayerTeam.Red)
            {
                message = "紅隊的回合";
            }
            else
            {
                message = "藍隊的回合";
            }
        }

        // dispay a message that it is player`s turn
        Sequence s = DOTween.Sequence();
        s.AppendInterval(0.4f);
        s.OnComplete(() => {
            new ShowMessageCommand(message, 2.0f).AddToQueue();
            player.DrawACard();
        });
    }
}
