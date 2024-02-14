using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CounterAttackOfForest : SpellEffect
{
    public CounterAttackOfForest()
    {
        hasTarget = true;
    }
    
    public override bool IsEffectTarget(Vector2Int tileIndex)
    {
        bool isTarget = false;
        CreatureLogic crl = ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y];
        if (crl != null)
        {
            isTarget = crl.owner.playerTeam != TurnManager.Instance.whoseTurn.playerTeam && !crl.IsKing;
        }
        return isTarget;
    }
    public override void ActivateEffect(List<Vector2Int> targetList = null)
    {
        int damage = 3;
        CreatureLogic crl = ChessboardManager.Instance.chessboard.creaturesOnTile[targetList[0].x, targetList[0].y];
        if (crl.GetTileAsset() == ChessboardManager.Instance.tileTypeForest)
        {
            damage = 6;
        }
        new UpdateCreatureHealthCommand(crl.ID, -damage, crl.Health - damage).AddToQueue();
        crl.Health -= damage;
    }
}