using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HydrangeaFairy : CreatureEffect
{ 
    public HydrangeaFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);
        
        List<Vector2Int> effectRange = creatureLogic.GetAttackRange();
        foreach (var tileIndex in effectRange)
        {
            ChessboardManager.Instance.chessboard.ChangeATile(tileIndex, ChessboardManager.Instance.tileTypeForest);
        }
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}