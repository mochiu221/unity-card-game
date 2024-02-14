using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoseFairy : CreatureEffect
{ 
    public RoseFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void DefenseEffect(CreatureLogic attacker)
    {
        int damage = creatureLogic.Attack * 2;
        new UpdateCreatureHealthCommand(attacker.ID, -(damage), attacker.Health - damage).AddToQueue();
        attacker.Health -= damage;
    }

    public override void OnTileEffect()
    {
        if (creatureLogic.GetTileAsset() == ChessboardManager.Instance.tileTypeForest)
        {
            creatureLogic.ChangeAllureStatus(true);
        }
        else
        {
            creatureLogic.ChangeAllureStatus(false);
        }
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}