using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GypsophilaFairy : CreatureEffect
{ 
    public GypsophilaFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);

        ChessboardManager.Instance.chessboard.ChangeATile(targetList[0], ChessboardManager.Instance.tileTypeForest);
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}