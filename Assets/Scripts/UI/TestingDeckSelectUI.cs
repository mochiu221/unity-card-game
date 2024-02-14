using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestingDeckSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private TextMeshProUGUI roomIDText;

    private void Awake() 
    {
        roomIDText.text = RoomHandler.Instance.GetRoomID().ToString();

        readyButton.onClick.AddListener(() => {
            RoomHandler.Instance.SetPlayerReady();
        });

        leaveButton.onClick.AddListener(() => {
            RoomHandler.Instance.LeaveARoom();
        });
    }
}
