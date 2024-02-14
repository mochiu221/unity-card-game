using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;

public class GameModeMultiplayerManager : NetworkBehaviour 
{
    public const int PLAYER_AMOUNT = 2;
    public static GameModeMultiplayerManager Instance { get; set; }

    public int thisPlayerID;

    public bool isHostPlayer;
    private bool isMyDeckShuffled;
    private bool isEnemyDeckShuffled;

    // Connecting
    public bool isConnectedNetwork;

    // Random number
    public int whoGoFirst;

    // temp number
    private int roomID;

    private void Awake() 
    {
        Instance = this;

        whoGoFirst = -1;

        isMyDeckShuffled = false;
        isEnemyDeckShuffled = false;
        isConnectedNetwork = false;

        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        thisPlayerID = 0;
    }

    // testing server
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void CreateARoom()
    {
        // testing - start
        thisPlayerID = 0;
        // testing - end

        if (!isConnectedNetwork)
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            RoomHandler.Instance.CreateARoom();
        }
        isHostPlayer = true;
    }

    public void JoinARoom(int roomID)
    {
        // testing - start
        thisPlayerID = 1;
        // testing - end

        if (!isConnectedNetwork)
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            RoomHandler.Instance.JoinARoom(roomID);
        }
        isHostPlayer = false;
        this.roomID = roomID;
    }

    public override void OnNetworkSpawn()
    {
        isConnectedNetwork = true;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    public override void OnNetworkDespawn()
    {
        isConnectedNetwork = false;
        RoomHandler.Instance.LeaveARoom();
    }

    // On first time connected, create room or join room
    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (!IsHost)
        {
            if (isHostPlayer)
            {
                RoomHandler.Instance.CreateARoom();
            }
            else
            {
                RoomHandler.Instance.JoinARoom(roomID);
            }
        }
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }

    // Flip coin 
    [ServerRpc(RequireOwnership = false)]
    public void FlipTheCoinServerRpc(int roomID)
    {
        int rnd = UnityEngine.Random.Range(0,2); // 0 or 1
        whoGoFirst = rnd;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = RoomHandler.Instance.playerReadyDict[roomID].Keys.ToArray()
            }
        };
        UpdateWhoGoFirstClientRpc(rnd, clientRpcParams);
        
    }

    [ClientRpc]
    public void UpdateWhoGoFirstClientRpc(int value, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        whoGoFirst = value;
    }

    // Shuffle deck
    public void ShuffleMyDeck()
    {
        System.Random rng = new System.Random();
        List<int> myDeckCardsOrder = new List<int>();

        // Randomize my deck order
        Player myPlayer = PlayersManager.Instance.myPlayer;
        int myDeckCount = myPlayer.deck.cards.Count;
        List<int> myOldDeckCardsOrder = new List<int>();
        for (int i = 0; i < myDeckCount; i++)
        {
            myOldDeckCardsOrder.Add(i);
        }
        while (myDeckCount > 0) 
        {
            myDeckCount--;  
            int k = rng.Next(myDeckCount + 1);  
            int value = myOldDeckCardsOrder[k];
            myOldDeckCardsOrder[k] = myOldDeckCardsOrder[myDeckCount];  
            myOldDeckCardsOrder[myDeckCount] = value;
            
            myDeckCardsOrder.Add(value);
        }

        // Shuffle my deck
        List<CardAsset> myTempDeckCards = new List<CardAsset>();

        for (int i = 0; i < myPlayer.deck.cards.Count; i++)
        {
            CardAsset ca = myPlayer.deck.cards[myDeckCardsOrder[i]];
            myTempDeckCards.Add(ca);
        }
        myPlayer.deck.cards = myTempDeckCards;

        isMyDeckShuffled = true;

        // Send to enemy
        string encodedMyDeckCardsOrder = EncodeListIntToStr(myDeckCardsOrder);
        // Debug.Log("enemy client id: "+RoomHandler.Instance.GetEnemyClientID());
        // Debug.Log("encodedMyDeckCardsOrder: "+encodedMyDeckCardsOrder);

        ShuffleEnemyDeckServerRpc(encodedMyDeckCardsOrder, RoomHandler.Instance.GetEnemyClientID());

        if (isEnemyDeckShuffled)
        {
            ResetVariablesOnGameStart();
            GameManager.Instance.StartAGame();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShuffleEnemyDeckServerRpc(string encodedEnemyDeckCardsOrder, ulong clientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientID}
            }
        };
        ShuffleEnemyDeckClientRpc(encodedEnemyDeckCardsOrder, clientRpcParams);
    }

    [ClientRpc]
    public void ShuffleEnemyDeckClientRpc(string encodedEnemyDeckCardsOrder, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;
        
        // Get deck order
        List<int> enemyDeckCardsOrder = DecodeStrToListInt(encodedEnemyDeckCardsOrder);

        Player enemyPlayer = PlayersManager.Instance.enemyPlayer;

        // Shuffle enemy deck
        List<CardAsset> enemyTempDeckCards = new List<CardAsset>();

        for (int i = 0; i < enemyPlayer.deck.cards.Count; i++)
        {
            CardAsset ca = enemyPlayer.deck.cards[enemyDeckCardsOrder[i]];
            enemyTempDeckCards.Add(ca);
        }
        enemyPlayer.deck.cards = enemyTempDeckCards;

        isEnemyDeckShuffled = true;

        if (isMyDeckShuffled)
        {
            ResetVariablesOnGameStart();
            GameManager.Instance.StartAGame();
        }
    }

    public string EncodeListIntToStr(List<int> list)
    {
        string str = "";
        foreach (var item in list)
        {
            if (item < 10)
            {
                str += "0";
            }
            str += item.ToString();
        }
        return str;
    }

    public List<int> DecodeStrToListInt(string str)
    {
        List<int> list = new List<int>();
        int listLength = str.Length/2;
        for (int i = 0; i < listLength; i++)
        {
            string subStr = str.Substring(i*2, 2);
            list.Add(int.Parse(subStr));
        }
        return list;
    }

    public void ResetVariablesOnGameStart()
    {
        whoGoFirst = -1;
        isMyDeckShuffled = false;
        isEnemyDeckShuffled = false;
    }

    // Turn manager
    public void UpdateWhoseTurnId(int id, ulong enemyClientID)
    {
        UpdateWhoseTurnIdServerRpc(id, enemyClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateWhoseTurnIdServerRpc(int id, ulong enemyClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{enemyClientID}
            }
        };
        UpdateWhoseTurnIdClientRpc(id, clientRpcParams);
    }

    [ClientRpc]
    private void UpdateWhoseTurnIdClientRpc(int id, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;
        
        TurnManager.Instance.WhoseTurnId = id;
    }

    // Draw card
    public void DrawACardAtIndex(int index, int ID)
    {
        if (ID != PlayersManager.Instance.myPlayer.ID) return;

        ulong myClientID = NetworkManager.Singleton.LocalClientId;
        ulong enemyClientID = RoomHandler.Instance.GetEnemyClientID();

        DrawACardAtIndexServerRpc(index, ID, myClientID, enemyClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DrawACardAtIndexServerRpc(int index, int ID, ulong myClientID, ulong enemyClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{myClientID, enemyClientID}
            }
        };
        DrawACardAtIndexClientRpc(index, ID, clientRpcParams);
    }

    [ClientRpc]
    private void DrawACardAtIndexClientRpc(int index, int ID, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        PlayersManager.Instance.GetPlayerById(ID).DrawACardAtIndex(index);
    }

    // Play creature
    public void PlayACreatureFromHand(int UniqueID, Vector2Int tileIndex, int targetInt, int ID)
    {
        ulong myClientID = NetworkManager.Singleton.LocalClientId;
        ulong enemyClientID = RoomHandler.Instance.GetEnemyClientID();

        PlayACreatureFromHandServerRpc(UniqueID, tileIndex, targetInt, ID, myClientID, enemyClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayACreatureFromHandServerRpc(int UniqueID, Vector2Int tileIndex, int targetInt, int ID, ulong myClientID, ulong enemyClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{myClientID, enemyClientID}
            }
        };
        PlayACreatureFromHandClientRpc(UniqueID, tileIndex, targetInt, ID, clientRpcParams);
    }

    [ClientRpc]
    private void PlayACreatureFromHandClientRpc(int UniqueID, Vector2Int tileIndex, int targetInt, int ID, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        PlayersManager.Instance.GetPlayerById(ID).PlayACreatureFromHandAction(UniqueID, tileIndex, targetInt);
    }

    // Play spell
    public void PlayASpellFromHand(int UniqueID, int targetInt, int ID)
    {
        ulong myClientID = NetworkManager.Singleton.LocalClientId;
        ulong enemyClientID = RoomHandler.Instance.GetEnemyClientID();

        PlayASpellFromHandServerRpc(UniqueID, targetInt, ID, myClientID, enemyClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayASpellFromHandServerRpc(int UniqueID, int targetInt, int ID, ulong myClientID, ulong enemyClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{myClientID, enemyClientID}
            }
        };
        PlayASpellFromHandClientRpc(UniqueID, targetInt, ID, clientRpcParams);
    }

    [ClientRpc]
    private void PlayASpellFromHandClientRpc(int UniqueID, int targetInt, int ID, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        PlayersManager.Instance.GetPlayerById(ID).PlayASpellFromHandAction(UniqueID, targetInt);
    }

    // Move Creature
    public void MoveCreature(Vector2Int currentIndex, Vector2Int newIndex)
    {
        ulong myClientID = NetworkManager.Singleton.LocalClientId;
        ulong enemyClientID = RoomHandler.Instance.GetEnemyClientID();

        MoveCreatureServerRpc(currentIndex, newIndex, myClientID, enemyClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveCreatureServerRpc(Vector2Int currentIndex, Vector2Int newIndex, ulong myClientID, ulong enemyClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{myClientID, enemyClientID}
            }
        };
        MoveCreatureClientRpc(currentIndex, newIndex, clientRpcParams);
    }

    [ClientRpc]
    private void MoveCreatureClientRpc(Vector2Int currentIndex, Vector2Int newIndex, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        ChessboardManager.Instance.chessboard.MoveCreatureAction(currentIndex, newIndex);
    }

    // Attack creature
    public void AttackCreature(Vector2Int currentIndex, Vector2Int newIndex)
    {
        ulong myClientID = NetworkManager.Singleton.LocalClientId;
        ulong enemyClientID = RoomHandler.Instance.GetEnemyClientID();

        AttackCreatureServerRpc(currentIndex, newIndex, myClientID, enemyClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackCreatureServerRpc(Vector2Int currentIndex, Vector2Int newIndex, ulong myClientID, ulong enemyClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{myClientID, enemyClientID}
            }
        };
        AttackCreatureClientRpc(currentIndex, newIndex, clientRpcParams);
    }

    [ClientRpc]
    private void AttackCreatureClientRpc(Vector2Int currentIndex, Vector2Int newIndex, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        ChessboardManager.Instance.chessboard.AttackCreatureAction(currentIndex, newIndex);
    }
}