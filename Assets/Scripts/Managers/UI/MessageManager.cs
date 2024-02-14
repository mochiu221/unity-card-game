using System.Collections;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { set; get; }

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private void Awake() 
    {
        Instance = this;
        messagePanel.SetActive(false);
    }

    public void ShowMessage(string message, float duration, Command command)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration, command));
    }

    IEnumerator ShowMessageCoroutine(string message, float duration, Command command)
    {
        messageText.text = message;
        messagePanel.SetActive(true);

        yield return new WaitForSeconds(duration);

        messagePanel.SetActive(false);
        Command.CommandExecutionComplete();
    }
}
