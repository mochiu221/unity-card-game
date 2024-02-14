using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SakuraFairy : CreatureEffect
{ 
    public SakuraFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);
        
        Chessboard cb = ChessboardManager.Instance.chessboard;
        List<CreatureLogic> targetCreatures = cb.GetCreaturesByTeam(owner.playerTeam);
        foreach (var crl in targetCreatures)
        {
            int attackBoost = 1;
            if (crl.cardAsset.creatureRace == CreatureRace.FlowerFairy)
            {
                attackBoost += 1;
            }
            if (cb.GetTileAssetByIndex(crl.TilePos) == ChessboardManager.Instance.tileTypeForest)
            {
                attackBoost += 1;
            }
            new UpdateCreatureAttackCommand(crl.ID, attackBoost, crl.Attack + attackBoost).AddToQueue();
            crl.Attack += attackBoost;
        }
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}