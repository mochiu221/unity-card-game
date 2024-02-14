using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Multiplayer,
    Computer
}

public class GameModeManager : MonoBehaviour 
{
    public static GameModeManager Instance { get; set; }

    private GameMode gameMode;

    private void Awake() 
    {
        Instance = this;

        gameMode = GameMode.Multiplayer;

        DontDestroyOnLoad(gameObject);
    }

    public void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    public GameMode GetGameMode()
    {
        // TODO: return game mode
        return gameMode;
    }

}