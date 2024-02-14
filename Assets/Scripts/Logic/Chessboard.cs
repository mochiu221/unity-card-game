using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chessboard : MonoBehaviour
{
    public CreatureLogic[,] creaturesOnTile = new CreatureLogic[3,3];
    public TileAsset[,] tileAssets = new TileAsset[3,3];

    // Move creature
    public void MoveCreature(Vector2Int currentIndex, Vector2Int newIndex)
    {
        GameModeMultiplayerManager.Instance.MoveCreature(currentIndex, newIndex);
    }

    public void MoveCreatureAction(Vector2Int currentIndex, Vector2Int newIndex)
    {
        CreatureLogic cl = creaturesOnTile[currentIndex.x, currentIndex.y];
        if (cl != null)
        {
            cl.TilePos = newIndex;
            // If position has changed, run creature move command
            if (cl.TilePos != currentIndex)
            {
                creaturesOnTile[newIndex.x, newIndex.y] = cl;
                creaturesOnTile[currentIndex.x, currentIndex.y] = null;
                if (cl.effect != null)
                {
                    cl.effect.OnTileEffect();
                }
                new CreatureMoveCommand(currentIndex, newIndex).AddToQueue();
            }
        }
    }

    // Attack creature
    public void AttackCreature(Vector2Int currentIndex, Vector2Int newIndex)
    {
        GameModeMultiplayerManager.Instance.AttackCreature(currentIndex, newIndex);
    }
    
    public void AttackCreatureAction(Vector2Int currentIndex, Vector2Int newIndex)
    {
        CreatureLogic crl = creaturesOnTile[currentIndex.x, currentIndex.y];
        CreatureLogic crlTarget = creaturesOnTile[newIndex.x, newIndex.y];
        if (crl != null && crlTarget != null)
        {
            crl.AttackCreature(crlTarget);
        }
    }

    // Change tile's world
    public void ChangeATile(Vector2Int tileIndex, TileAsset tileAsset)
    {
        tileAssets[tileIndex.x, tileIndex.y] = tileAsset;
        if (creaturesOnTile[tileIndex.x, tileIndex.y] != null)
        {
            if (creaturesOnTile[tileIndex.x, tileIndex.y].effect != null)
            {
                creaturesOnTile[tileIndex.x, tileIndex.y].effect.OnTileEffect();
            }
        }
        new ChangeTileCommand(tileIndex, tileAsset).AddToQueue();
    }

    public TileAsset GetTileAssetByIndex(Vector2Int tileIndex)
    {
        return tileAssets[tileIndex.x, tileIndex.y];
    }

    public List<CreatureLogic> GetCreaturesByTeam(PlayerTeam pt)
    {
        List<CreatureLogic> creaturesList = new List<CreatureLogic>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                CreatureLogic crl = creaturesOnTile[x,y];
                if (crl != null && !crl.IsKing && crl.owner.playerTeam == pt)
                {
                    creaturesList.Add(crl);
                }
            }
        }
        return creaturesList;
    }
    public CreatureLogic GetKingByTeam(PlayerTeam pt)
    {
        CreatureLogic king = null;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                CreatureLogic crl = creaturesOnTile[x,y];
                if (crl != null && crl.IsKing && crl.owner.playerTeam == pt)
                {
                    king = crl;
                    break;
                }
            }
        }
        return king;
    }

    // Can the creature move to this tile?
    public bool IsAValidTileToMove(Vector2Int tileIndex)
    {
        return creaturesOnTile[tileIndex.x, tileIndex.y] == null;
    }

    // Can the creature attack to this tile creature?
    public bool IsAValidTileToAttack(Vector2Int tileIndex, PlayerTeam attackTeam)
    {
        bool canBeAttack = false;
        CreatureLogic crl = creaturesOnTile[tileIndex.x, tileIndex.y];
        if (crl != null)
            canBeAttack = !crl.hiding && crl.owner.playerTeam != attackTeam;
        
        return canBeAttack;
    }
        
}