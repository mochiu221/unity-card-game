using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LilacFairy : CreatureEffect
{ 
    public LilacFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);
        
        owner.DrawARandomCardByRace(CreatureRace.FlowerFairy);
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}