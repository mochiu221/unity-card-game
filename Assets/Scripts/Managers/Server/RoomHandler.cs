using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class RoomHandler : NetworkBehaviour
{
    public static RoomHandler Instance { get; private set; }

    // All rooms
    public const int MAX_ROOM_COUNT = 90000;
    private int myRoomID;
    private ulong enemyClientID;

    // Host
    private List<int> freeRooms;
    public Dictionary<int, Dictionary<ulong, bool>> playerReadyDict; // room id => { client id => ready? }

    private void Awake() 
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitRooms();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        for (int i = 0; i < MAX_ROOM_COUNT; i++)
        {
            freeRooms.Add(i + 10000);
        }
    }

    private void InitRooms()
    {
        freeRooms = new List<int>();
        playerReadyDict = new Dictionary<int, Dictionary<ulong, bool>>();
    }

    public void CreateARoom()
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        CreateARoomServerRpc(clientID);
    }

    public void JoinARoom(int roomID)
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        JoinARoomServerRpc(roomID, clientID);
    }

    public void LeaveARoom()
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        LeaveARoomServerRpc(myRoomID, clientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateARoomServerRpc(ulong clientID)
    {
        int rnd = -1;
        if (freeRooms.Count > 0)
        {
            int rndIndex = Random.Range(0,freeRooms.Count);
            rnd = freeRooms[rndIndex];
            freeRooms.RemoveAt(rndIndex);
        }
        else
        {
            Debug.Log("No empty room now.");
        }

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientID}
            }
        };

        JoinARoomClientRpc(rnd, clientID, clientRpcParams);
    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinARoomServerRpc(int roomID, ulong clientID)
    {
        if (roomID == -1)
        {
            // TODO: room filled message
            Debug.Log("No free room now");
            return;
        }

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientID}
            }
        };

        if (playerReadyDict.ContainsKey(roomID))
        {
            if (playerReadyDict[roomID].Count < 2)
            {
                JoinARoomClientRpc(roomID, clientID, clientRpcParams);
            }
            else
            {
                // TODO: popup message
                Debug.Log("Maximum 2 players in one room!");
            }
        }
        else
        {
            // TODO: popup message
            Debug.Log("Room ID does not exists.");
        }
    }

    [ClientRpc]
    private void JoinARoomClientRpc(int roomID, ulong clientID, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        myRoomID = roomID;
        AddPlayerReadyDictServerRpc(myRoomID, clientID);
        MultiplayerSceneManager.Instance.ChangeScene(Scene.DeckSelectScene);
    }

    [ServerRpc(RequireOwnership = false)] 
    public void AddPlayerReadyDictServerRpc(int roomID, ulong clientID)
    {
        Dictionary<ulong, bool> players = new Dictionary<ulong, bool>();
        if (playerReadyDict.ContainsKey(roomID))
        {
            players = playerReadyDict[roomID];
            players.Add(clientID, false);
            playerReadyDict[roomID] = players;
        }
        else
        {
            players.Add(clientID, false);
            playerReadyDict.Add(roomID, players);
        }
        
        // Debug.Log("host rpc - room id dict: "+ playerReadyDict);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveARoomServerRpc(int roomID, ulong clientID)
    {
        if (!playerReadyDict.ContainsKey(roomID)) return;

        playerReadyDict[roomID].Remove(clientID);

        if (playerReadyDict[roomID].Count == 0)
        {
            playerReadyDict.Remove(roomID);
            freeRooms.Add(roomID);
        }
        else
        {
            ulong enemyClientID = 0;
            Dictionary<ulong, bool> tempDict = playerReadyDict[roomID];

            foreach (var p in tempDict)
            {
                if (p.Key != clientID)
                {
                    enemyClientID = p.Key;
                }
            }

            ClientRpcParams enemyClientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]{enemyClientID}
                }
            };
            UpdateRoomHostClientRpc(enemyClientRpcParams);
        }

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientID}
            }
        };

        LeaveARoomClientRpc(clientID, clientRpcParams);

    }

    [ClientRpc]
    private void LeaveARoomClientRpc(ulong clientID, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        myRoomID = -1;
        MultiplayerSceneManager.Instance.ChangeScene(Scene.LobbyScene);
    }

    [ClientRpc]
    private void UpdateRoomHostClientRpc(ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        GameModeMultiplayerManager.Instance.isHostPlayer = true;
    }

    // Set Ready
    public void SetPlayerReady()
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        SetPlayerReadyServerRpc(myRoomID, clientID);
    }

    public void UnsetPlayerReady()
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        UnsetPlayerReadyServerRpc(myRoomID, clientID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(int roomID, ulong clientID)
    {
        bool isAllPlayersReady = true;
        if (playerReadyDict[roomID].ContainsKey(clientID))
        {
            playerReadyDict[roomID][clientID] = true;
        }

        foreach (var playerReady in playerReadyDict[roomID])
        {
            if (!playerReady.Value)
            {
                isAllPlayersReady = false;
            }
        }
        // Debug.Log("player count: "+playerReadyDict[roomID].Count);
        // Debug.Log("isAllPlayersReady: "+isAllPlayersReady);
        if (playerReadyDict[roomID].Count == 2 && isAllPlayersReady)
        {
            GameModeMultiplayerManager.Instance.FlipTheCoinServerRpc(roomID);
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = playerReadyDict[roomID].Keys.ToArray()
                }
            };

            Dictionary<ulong, bool> tempDict = new Dictionary<ulong, bool>();
            foreach (var key in playerReadyDict[roomID].Keys)
            {
                tempDict.Add(key, false);
            }
            playerReadyDict[roomID] = tempDict;

            StartPlayerGameClientRpc(clientRpcParams);
        }
    }
    
    [ClientRpc]
    public void StartPlayerGameClientRpc(ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        UpdateEnemyClientID();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnsetPlayerReadyServerRpc(int roomID, ulong clientID)
    {
        if (playerReadyDict[roomID].ContainsKey(clientID))
        {
            playerReadyDict[roomID][clientID] = false;
        }
    }

    public void UpdateEnemyClientID()
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        UpdateEnemyClientIDServerRpc(myRoomID, clientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateEnemyClientIDServerRpc(int roomID, ulong clientID)
    {
        if (!playerReadyDict.ContainsKey(roomID)) return;

        ulong enemyClientID = 0;
        Dictionary<ulong, bool> tempDict = playerReadyDict[roomID];

        foreach (var p in tempDict)
        {
            if (p.Key != clientID)
            {
                enemyClientID = p.Key;
            }
        }

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientID}
            }
        };

        UpdateEnemyClientIDClientRpc(enemyClientID, clientRpcParams);
    }

    [ClientRpc]
    private void UpdateEnemyClientIDClientRpc(ulong enemyClientID, ClientRpcParams clientRpcParams)
    {
        if (IsOwner) return;

        this.enemyClientID = enemyClientID;
        MultiplayerSceneManager.Instance.ChangeScene(Scene.BattleScene);
    }
    

    // Get
    public int GetRoomID()
    {
        return myRoomID;
    }

    public ulong GetEnemyClientID()
    {
        return enemyClientID;
    }
}