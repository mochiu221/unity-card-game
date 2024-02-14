using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fireball : SpellEffect 
{
    public Fireball()
    {
        hasTarget = true;
    }
    
    public override bool IsEffectTarget(Vector2Int tileIndex)
    {
        return ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y].owner.playerTeam != TurnManager.Instance.whoseTurn.playerTeam;
    }
    public override void ActivateEffect(List<Vector2Int> targetList = null)
    {
        int damage = 2;
        foreach (var tileIndex in targetList)
        {
            CreatureLogic target = ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y];
            new UpdateCreatureHealthCommand(target.ID, -damage, target.Health - damage).AddToQueue();
            target.Health -= damage;
        }
    }
}