using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createNewRoomButton;
    [SerializeField] private Button joinARoomButton;
    [SerializeField] private TMP_InputField roomIDInput;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject PopupUI;

    // test server
    [SerializeField] private Button hostButton;

    private void Awake() 
    {
        // test server
        hostButton.onClick.AddListener(() => {
            GameModeMultiplayerManager.Instance.StartHost();
        });

        PopupUI.SetActive(false);

        createNewRoomButton.onClick.AddListener(() => {
            GameModeMultiplayerManager.Instance.CreateARoom();
        });

        joinARoomButton.onClick.AddListener(() => {
            PopupUI.SetActive(true);
        });

        cancelButton.onClick.AddListener(() => {
            PopupUI.SetActive(false);
            roomIDInput.text = "";
        });

        enterButton.onClick.AddListener(() => {
            GameModeMultiplayerManager.Instance.JoinARoom(int.Parse(roomIDInput.text.ToString()));
        });
    }

}
