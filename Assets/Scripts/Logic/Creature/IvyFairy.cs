using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IvyFairy : CreatureEffect
{ 
    public IvyFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);
        
        ChessboardManager.Instance.chessboard.ChangeATile(creatureLogic.TilePos, ChessboardManager.Instance.tileTypeForest);
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}