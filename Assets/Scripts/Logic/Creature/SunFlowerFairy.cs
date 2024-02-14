using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunFlowerFairy : CreatureEffect
{ 
    public SunFlowerFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);
        
        if (creatureLogic.GetTileAsset() == ChessboardManager.Instance.tileTypeForest)
        {
            creatureLogic.AttacksLeftThisTurn = creatureLogic.cardAsset.attacksForOneTurn;
        }
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}