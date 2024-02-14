using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerFairysPray : SpellEffect 
{
    public FlowerFairysPray()
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
        CreatureLogic crl = ChessboardManager.Instance.chessboard.creaturesOnTile[targetList[0].x, targetList[0].y];
        int newMaxHealth = crl.MaxHealth + 4;
        new UpdateCreatureHealthCommand(crl.ID, 4, newMaxHealth).AddToQueue();
        crl.MaxHealth = newMaxHealth;
        crl.Health = newMaxHealth;
    }
}