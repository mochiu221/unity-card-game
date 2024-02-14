using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using System;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    private TurnTimerManager timer;

    private Player playerRed;
    private Player playerBlue;

    private int initDraw = 3;
    private int initMana = 3;
    
    private int whoseTurnId;
    public int WhoseTurnId
    {
        get { return whoseTurnId; }
        set 
        {
            whoseTurnId = value;

            Player player = PlayersManager.Instance.GetPlayerById(value);
            _whoseTurn = player;

            if (player == PlayersManager.Instance.myPlayer)
            {
                timer.StartTimer();
            }
            
            PlayersManager.Instance.EnableEndTurnButtonOnStart(player);

            TurnMaker tm = player.GetComponent<TurnMaker>();
            // player`s method OnTurnStart() will be called in tm.OnTurnStart();
            tm.OnTurnStart();
            if (tm is PlayerTurnMaker)
            {
                player.HighlightPlayableCards();
            }
            // remove highlights for opponent.
            player.otherPlayer.HighlightPlayableCards(true);
        }
    }
    private Player _whoseTurn;
    public Player whoseTurn
    {
        get { return _whoseTurn; }
        set
        {
            UpdateWhoseTurnId(value.ID);
        }
    }

    public int turnCount = 0;

    private void Awake() 
    {
        Instance = this;
        timer = GetComponent<TurnTimerManager>();

        whoseTurnId = -1;
    }

    private void Start() 
    {
        UIManager.Instance.beforeStartCharPreview.SetActive(true);
        UIManager.Instance.myTeamCharImage.sprite = PlayersManager.Instance.myPlayer.charAsset.charIllustration;
        UIManager.Instance.enemyTeamCharImage.sprite = PlayersManager.Instance.enemyPlayer.charAsset.charIllustration;
    }

    // update my WhoseTurnId
    public void UpdateWhoseTurnId(int id)
    {
        ulong enemyClientID = RoomHandler.Instance.GetEnemyClientID();

        WhoseTurnId = id;
        GameModeMultiplayerManager.Instance.UpdateWhoseTurnId(id, enemyClientID);
    }

    public void OnGameStart()
    {
        Debug.Log("game start");
        //Debug.Log("In TurnManager.OnGameStart()");

        CardLogic.CardsCreatedThisGame.Clear();
        CreatureLogic.CreaturesCreatedThisGame.Clear();


        List<Player> allPlayers = new List<Player>();
        allPlayers.Add(PlayersManager.Instance.myPlayer);
        allPlayers.Add(PlayersManager.Instance.enemyPlayer);
        foreach (Player p in allPlayers)
        {
            p.ManaThisTurn = initMana;
            p.ManaLeft = initMana;
            p.LoadCharacterInfoFromAsset();
            p.TransmitInfoAboutPlayerToVisual();
            p.playerArea.deckVisual.CardsInDeck = p.deck.cards.Count;
        }

        playerRed = PlayersManager.Instance.PlayersByTeam[PlayerTeam.Red];
        playerBlue = PlayersManager.Instance.PlayersByTeam[PlayerTeam.Blue];

        playerRed.king.transform.localPosition = new Vector3(0, 0, -1f);
        playerBlue.king.transform.localPosition = new Vector3(0, 0, -1f);

        // Start animation
        Sequence s = DOTween.Sequence();

        s.AppendInterval(2f);
        s.Append(UIManager.Instance.battleSlogan.DOFade(0, 0.2f));
        s.Append(UIManager.Instance.enemyTeamCharImagePanel.transform.DOLocalMoveY(UIManager.Instance.enemyTeamCharImagePanel.transform.localPosition.y + 200f, 1f));
        s.Join(UIManager.Instance.myTeamCharImagePanel.transform.DOLocalMoveY(UIManager.Instance.myTeamCharImagePanel.transform.localPosition.y - 200f, 1f));
        s.Join(UIManager.Instance.enemyTeamCharImage.DOFade(0, 1f));
        s.Join(UIManager.Instance.myTeamCharImage.DOFade(0, 1f));
        s.AppendCallback(() => {
            UIManager.Instance.beforeStartCharPreview.SetActive(false);
        });

        s.Append(playerRed.king.transform.DOLocalMoveZ(0, 0.3f).SetEase(Ease.InBack));
        s.Join(playerBlue.king.transform.DOLocalMoveZ(0, 0.3f).SetEase(Ease.InBack));
        s.AppendInterval(1.2f);
        s.OnComplete(() =>
            {
                for (int i = 0; i < initDraw; i++)
                {            
                    // first player draws a card
                    playerRed.DrawACard(true);
                    // second player draws a card
                    playerBlue.DrawACard(true);
                }
                if (playerRed == PlayersManager.Instance.myPlayer)
                {
                    new StartATurnCommand(playerRed).AddToQueue();
                }
            }
        );
    }

    public void EndTurn()
    {
        // stop timer
        timer.StopTimer();
        // Disable the end turn button to prevent double click
        PlayersManager.Instance.DisableEndTurnButton();
        // send all commands in the end of current player`s turn
        whoseTurn.OnTurnEnd();
        new StartATurnCommand(whoseTurn.otherPlayer).AddToQueue();
    }

    public void FreezeTurn()
    {
        timer.StopTimer();
    }
}
