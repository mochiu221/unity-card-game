using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireFlowerFairy : CreatureEffect
{ 
    public FireFlowerFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void OnDraggingBeingResummonedTargetEffect(CardLogic cl)
    {
        if (cl.cardAsset.creatureRace == CreatureRace.FlowerFairy)
        {
            creatureLogic.resummonCostReduction = 5;
        }
        else
        {
            creatureLogic.resummonCostReduction = creatureLogic.BaseResummonCostReduction;
        }
    }

    public override void StopOnDraggingCardEffect(CardLogic cl)
    {
        creatureLogic.resummonCostReduction = creatureLogic.BaseResummonCostReduction;
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }

    public override int GetEffectCostChangeToCard(CardLogic cl)
    {
        if (cl.cardAsset.creatureRace == CreatureRace.FlowerFairy)
        {
            return -( 5 - creatureLogic.BaseResummonCostReduction );
        }
        return 0;
    }
}