using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CreatureEffect 
{
    protected Player owner;
    protected CreatureLogic creatureLogic;

    public CreatureEffect(Player owner, CreatureLogic creatureLogic)
    {
        this.creatureLogic = creatureLogic;
        this.owner = owner;
    }

    public virtual void OnDraggingBeingResummonedTargetEffect(CardLogic cl){}
    public virtual void StopOnDraggingCardEffect(CardLogic cl){}

    public virtual void SummonEffect(List<Vector2Int> targetList){
        OnTileEffect();
    }

    public virtual void AttackEffect(){}

    public virtual void DefenseEffect(CreatureLogic attacker){}

    public virtual void ContinuousEffect(){}

    public abstract void RegisterEffect();

    public abstract void CauseEffect();

    public virtual int GetEffectCostChangeToCard(CardLogic cl){ return 0; }

    public virtual void OnTileEffect(){}

}
