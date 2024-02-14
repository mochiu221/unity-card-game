using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSceneManager : MonoBehaviour
{
    public static MultiplayerSceneManager Instance { get; private set; }

    [SerializeField] private GameObject lobbyScene;
    [SerializeField] private GameObject deckSelectScene;
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject battleScene;

    public GameObject currentScene;

    private void Awake() 
    {
        Instance = this;

        currentScene = GameObject.Instantiate(lobbyScene) as GameObject;
    }

    public void ChangeScene(Scene scene)
    {
        GameObject oldScene = currentScene;
        switch (scene)
        {
            case Scene.LobbyScene:
                currentScene = GameObject.Instantiate(lobbyScene) as GameObject;
                Destroy(oldScene);
                break;

            case Scene.DeckSelectScene:
                currentScene = GameObject.Instantiate(deckSelectScene) as GameObject;
                Destroy(oldScene);
                break;

            case Scene.LoadingScene:
                currentScene = GameObject.Instantiate(loadingScene) as GameObject;
                Destroy(oldScene);
                break;

            case Scene.BattleScene:
                currentScene = GameObject.Instantiate(battleScene) as GameObject;
                Destroy(oldScene);
                break;

            default:
                break;
        }
    }
}
