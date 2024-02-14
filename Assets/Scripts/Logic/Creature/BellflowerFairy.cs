using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BellflowerFairy : CreatureEffect
{ 
    public BellflowerFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);
        
        owner.DrawACard();
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}