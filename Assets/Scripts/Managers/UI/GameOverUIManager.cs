using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button backToTheRoomButton;

    string messageText = "";

    private void Awake() 
    {
        backToTheRoomButton.onClick.AddListener(() => {
            MultiplayerSceneManager.Instance.ChangeScene(Scene.DeckSelectScene);
        });

        Hide();
    }

    private void Start() 
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
            if (PlayersManager.Instance.myPlayer == GameManager.Instance.loser)
            {
                messageText = "你輸了...";
            }
            else
            {
                messageText = "你贏了!!";
            }
            message.text = messageText;
        }
    }

    private void Show()
    {
        gameOverUI.SetActive(true);
    }

    private void Hide()
    {
        gameOverUI.SetActive(false);
    }
}
