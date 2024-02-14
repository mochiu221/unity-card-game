using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ETargetAllTiles : SpellEffect 
{
    public ETargetAllTiles()
    {
        hasTarget = true;
    }
    
    public override bool IsEffectTarget(Vector2Int tileIndex)
    {
        return true;
    }
}