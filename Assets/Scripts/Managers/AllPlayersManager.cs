using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPlayersManager : MonoBehaviour
{
    public static AllPlayersManager Instance { get; set; }

    // TODO: Should get player from DB, this only for testing
    [SerializeField] private List<PlayerAsset> playerAssetList;

    private void Awake() 
    {
        Instance = this;
    }

    public CharacterAsset GetPlayerCharByID(int ID)
    {
        CharacterAsset ca = playerAssetList[ID].charAsset;
        return ca;
    }

    public List<CardAsset> GetPlayerDeckByID(int ID)
    {
        List<CardAsset> deck = new List<CardAsset>(playerAssetList[ID].deck);
        return deck;
    }

}