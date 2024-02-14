using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellEffect
{
    public bool hasTarget = false;
    
    public virtual bool IsEffectTarget(Vector2Int tileIndex)
    {
        return false;
    }
    public virtual void ActivateEffect(List<Vector2Int> targetList = null)
    {
        
    }
        
}