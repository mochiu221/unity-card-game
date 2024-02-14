using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Start() 
    {
        startHostButton.onClick.AddListener(() => {
            Debug.Log("start host");
            NetworkManager.Singleton.StartHost();
            Hide();
            GameManager.Instance.StartAGame();
        });

        startClientButton.onClick.AddListener(() => {
            Debug.Log("start client");
            NetworkManager.Singleton.StartClient();
            Hide();
            GameManager.Instance.StartAGame();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
