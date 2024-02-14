using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForestOfFlowerFairys : SpellEffect 
{
    public override void ActivateEffect(List<Vector2Int> targetList = null)
    {
        Chessboard cb = ChessboardManager.Instance.chessboard;
        TileAsset forest = ChessboardManager.Instance.tileTypeForest;
        PlayerTeam myTeam = TurnManager.Instance.whoseTurn.playerTeam;
        PlayerTeam otherTeam = TurnManager.Instance.whoseTurn.otherPlayer.playerTeam;

        int damage = 1;
        int fairyNo = 0;
        int forestNo = 0;

        List<CreatureLogic> crlList = cb.GetCreaturesByTeam(myTeam);
        foreach (var crl in crlList)
        {
            if (crl.cardAsset.creatureRace == CreatureRace.FlowerFairy)
            {
                fairyNo ++;
            }
        }
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (cb.GetTileAssetByIndex(new Vector2Int(x,y)) == forest)
                {
                    forestNo ++;
                }
            }
        }
        damage += fairyNo * forestNo;

        List<CreatureLogic> targets = cb.GetCreaturesByTeam(otherTeam);
        foreach (var crl in targets)
        {
            new UpdateCreatureHealthCommand(crl.ID, -damage, crl.Health - damage).AddToQueue();
            crl.Health -= damage;
        }
        CreatureLogic otherKing = cb.GetKingByTeam(otherTeam);
        new UpdateCreatureHealthCommand(otherKing.ID, -damage, otherKing.Health - damage).AddToQueue();
        otherKing.Health -= damage;
        
    }
}