using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { set; get; }

    [Header("Prefabs")]
    public GameObject creatureCardPrefab;
    public GameObject spellCardPrefab;
    public GameObject kingPrefab;
    public GameObject creaturePrefab;

    [Header("Settings")]
    public float cardPreviewTime = 1f;
    public float cardTransitionTime= 1f;
    public float cardPreviewTimeFast = 0.2f;
    public float cardTransitionTimeFast = 0.5f;

    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameOver
    }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;

    private State state;
    private State GameState 
    {
        get { return state; }
        set 
        {
            State previousState = state;
            state = value;
            GameState_OnValueChanged(previousState, state);
        }
    }
    // private float waitingToStartTimer = 1f;
    private bool isLocalPlayerReady;

    public Player winner;
    public Player loser;

    private void Awake() 
    {
        Instance = this;
    }

    private void GameState_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start() 
    {
        isLocalPlayerReady = true;
        OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Update() 
    {
        if (!IsServer) return;

        switch (state)
        {
            case State.WaitingToStart:
                state = State.GamePlaying;
                break;

            case State.GamePlaying:
                break;

            case State.GameOver:
                break;
            
            default:
                break;
        }
    }

    public void StartAGame()
    {
        TurnManager.Instance.OnGameStart();
    }

    public void StopAGame()
    {
        TurnManager.Instance.FreezeTurn();
    }

    public void RestartAGame()
    {

    }

    public void GameOver(Player loser)
    {
        StopAGame();
        this.loser = loser;
        winner = loser.otherPlayer;
        GameState = State.GameOver;
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsWaitingToStart()
    {
        return state == State.WaitingToStart;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
}