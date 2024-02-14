using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LilyFairy : CreatureEffect
{ 
    public LilyFairy(Player owner, CreatureLogic creatureLogic): base(owner, creatureLogic)
    {
        
    }

    public override void SummonEffect(List<Vector2Int> targetList)
    {
        base.SummonEffect(targetList);

        List<Vector2Int> effectRange = creatureLogic.GetValidTilesToAttackOrMove();
        int heal = 3;
        foreach (var tileIndex in effectRange)
        {
            CreatureLogic crl = ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y];
            if (crl != null && crl.owner.playerTeam == TurnManager.Instance.whoseTurn.playerTeam)
            {
                int healthAfter = crl.Health + heal > crl.MaxHealth ? crl.MaxHealth : crl.Health + heal;
                new UpdateCreatureHealthCommand(crl.ID, heal, healthAfter).AddToQueue();
                crl.Health += heal;
            }
        }
    }

    public override void OnTileEffect()
    {
        if (creatureLogic.GetTileAsset() == ChessboardManager.Instance.tileTypeForest)
        {
            creatureLogic.ChangeHidingStatus(true);
        }
        else
        {
            creatureLogic.ChangeHidingStatus(false);
        }
    }

    public override void RegisterEffect()
    {
        
    }

    public override void CauseEffect()
    {
        
    }
}