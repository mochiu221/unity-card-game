using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerOfForestation : SpellEffect
{
    public FlowerOfForestation()
    {
        hasTarget = true;
    }
    
    public override bool IsEffectTarget(Vector2Int tileIndex)
    {
        bool isTarget = false;
        CreatureLogic crl = ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y];
        if (crl != null)
        {
            isTarget = crl.owner.playerTeam == TurnManager.Instance.whoseTurn.playerTeam && !crl.IsKing && crl.cardAsset.creatureRace == CreatureRace.FlowerFairy;
        }
        return isTarget;
    }
    public override void ActivateEffect(List<Vector2Int> targetList = null)
    {
        ChessboardManager.Instance.chessboard.ChangeATile(targetList[0], ChessboardManager.Instance.tileTypeForest);
    }
}