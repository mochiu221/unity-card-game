using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance { set; get; }

    public GameMode gameMode;

    [Header("Players")]
    public bool isTesting = true; // Testing mode
    public int playerTestingID = 0; // Test if I am player1 / player2

    // Set players
    public Player hostPlayer;
    public Player clientPlayer;

    public Player myPlayer;
    public Player enemyPlayer;
    public Dictionary<PlayerTeam, Player> PlayersByTeam = new Dictionary<PlayerTeam, Player>();

    public PlayerArea myArea;
    public PlayerArea enemyArea;

    public HandManager myHandManager;
    public HandManager enemyHandManager;

    // deck orders
    public List<int> myDeckCardsOrder;
    public List<int> enemyDeckCardsOrder;

    [Header("Colors")]
    public Color32 redTeamCreatureFrameColor;
    public Color32 blueTeamCreatureFrameColor;

    [Header("Button")]
    public Button endTurnButton;


    private void Awake() 
    {
        myDeckCardsOrder = new List<int>();
        enemyDeckCardsOrder = new List<int>();

        InitPlayers();
        AssignPlayersTeam();
        
        Instance = this;
    }

    private void Start() 
    {
        GameModeMultiplayerManager.Instance.ShuffleMyDeck();
    }

    public void InitPlayers()
    {
        hostPlayer.playerID = 0;
        clientPlayer.playerID = 1;

        if (!isTesting)
        {
            gameMode = GameModeManager.Instance.GetGameMode();
        }
        else
        {
            gameMode = GameMode.Computer;
        }

        if (!isTesting)
        {
            if (gameMode == GameMode.Multiplayer)
            {
                // Init player's ID
                if (GameModeMultiplayerManager.Instance.isHostPlayer)
                {
                    // I am host player
                    myPlayer = hostPlayer;
                    enemyPlayer = clientPlayer;
                }
                else
                {
                    // I am client player
                    myPlayer = clientPlayer;
                    enemyPlayer = hostPlayer;
                }
                // Player Area
                myPlayer.playerArea = myArea;
                enemyPlayer.playerArea = enemyArea;

                // Hand Manager
                myHandManager.player = myPlayer;
                enemyHandManager.player = enemyPlayer;

                // Init data
                myPlayer.charAsset = AllPlayersManager.Instance.GetPlayerCharByID(myPlayer.playerID);
                myPlayer.deck.cards = AllPlayersManager.Instance.GetPlayerDeckByID(myPlayer.playerID);
                enemyPlayer.charAsset = AllPlayersManager.Instance.GetPlayerCharByID(enemyPlayer.playerID);
                enemyPlayer.deck.cards = AllPlayersManager.Instance.GetPlayerDeckByID(enemyPlayer.playerID);
            }
        }
    }

    public void AssignPlayersTeam()
    {
        // Determine who starts the game
        // players[0] will be assigned to red team, red team go first
        int rnd;
        int rndOther;
        if (gameMode == GameMode.Multiplayer)
        {
            rnd = GameModeMultiplayerManager.Instance.whoGoFirst;
        }
        else
        {
            rnd = Random.Range(0,2); // 0 or 1
        }
        rndOther = rnd == 0 ? 1 : 0;

        if (isTesting)
        {
            rnd = 0; rndOther = 1;
        }

        // Init player's team
        if (myPlayer.ID == rnd)
        {
            myPlayer.playerTeam = PlayerTeam.Red;
            enemyPlayer.playerTeam = PlayerTeam.Blue;
            // Add to manager dictionary
            PlayersByTeam.Add(PlayerTeam.Red, myPlayer);
            PlayersByTeam.Add(PlayerTeam.Blue, enemyPlayer);
        }
        else
        {
            myPlayer.playerTeam = PlayerTeam.Blue;
            enemyPlayer.playerTeam = PlayerTeam.Red;
            // Add to manager dictionary
            PlayersByTeam.Add(PlayerTeam.Blue, myPlayer);
            PlayersByTeam.Add(PlayerTeam.Red, enemyPlayer);
        }
    }

    public bool CanViewThisPlayerHand(PlayerTeam team)
    {
        bool IsPlayerHand = false;

        if (gameMode == GameMode.Multiplayer)
        {
            IsPlayerHand = myPlayer == PlayersByTeam[team];
        }

        if (isTesting)
        {
            IsPlayerHand = (PlayersByTeam[team].ID == playerTestingID);
        }
        return IsPlayerHand;
    }

    public bool CanViewThisPlayerHand(Player ownerPlayer)
    {
        bool IsPlayerHand = false;

        if (gameMode == GameMode.Multiplayer)
        {
            IsPlayerHand = myPlayer == ownerPlayer;
        }

        if (isTesting)
        {
            IsPlayerHand = (ownerPlayer.ID == playerTestingID);
        }
        return IsPlayerHand;
    }

    public bool CanControlThisPlayer(PlayerTeam team)
    {
        return CanControlThisPlayer(PlayersByTeam[team]);
    }

    public bool CanControlThisPlayer(Player ownerPlayer)
    {
        bool PlayersTurn = (TurnManager.Instance.whoseTurn == ownerPlayer);

        if (gameMode == GameMode.Multiplayer)
        {
            PlayersTurn = PlayersTurn && myPlayer == ownerPlayer;
        }

        if (isTesting)
        {
            PlayersTurn = (PlayersTurn && ownerPlayer.ID == playerTestingID);
        }
        bool NotDrawingAnyCards = !Command.CardDrawPending();
        return ownerPlayer.playerArea.AllowedToControlThisPlayer && ownerPlayer.playerArea.controlsON && PlayersTurn && NotDrawingAnyCards;
    }

    public bool CanEndTurn(PlayerTeam team)
    {
        Player ownerPlayer = PlayersByTeam[team];
        bool PlayersTurn = (TurnManager.Instance.whoseTurn == ownerPlayer);

        if (gameMode == GameMode.Multiplayer)
        {
            PlayersTurn = PlayersTurn && myPlayer == ownerPlayer;
        }

        if (isTesting)
        {
            PlayersTurn = (PlayersTurn && ownerPlayer.ID == playerTestingID);
        }
        return ownerPlayer.playerArea.AllowedToControlThisPlayer && ownerPlayer.playerArea.controlsON && PlayersTurn;
    }

    public void EnableEndTurnButtonOnStart(Player player)
    {
        if (player == PlayersByTeam[PlayerTeam.Red] && CanEndTurn(PlayerTeam.Red) ||
            player == PlayersByTeam[PlayerTeam.Blue] && CanEndTurn(PlayerTeam.Blue))
            endTurnButton.interactable = true;
        else
            endTurnButton.interactable = false;
    }

    public void DisableEndTurnButton()
    {
        endTurnButton.interactable = false;
    }

    public Player GetPlayerById(int id)
    {
        if (id == myPlayer.ID)
        {
            return myPlayer;
        }
        else
        {
            return enemyPlayer;
        }
    }
}
