using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class TurnTimerManager : MonoBehaviour, IEventSystemHandler
{
    [SerializeField] private Image timerFillImage;
    [SerializeField] private float fillFraction = 1f;
	[SerializeField] private float timeForOneTurn = 120f;
    [SerializeField] public TextMeshProUGUI timerText;

    private float timeTillZero;
    private bool counting = false;

    [SerializeField]
    public UnityEvent TimerExpired = new UnityEvent();

    private void Awake()
    {
		if (timerText!=null)
            timerText.text = "";

        if (timerFillImage != null)
        	timerFillImage.fillAmount = 1;
        
    }

    public void StartTimer()
	{
        if (timerText != null)
            timerText.text = ToString();

		if (timerFillImage != null)
        	timerFillImage.fillAmount = 1;

        timeTillZero = timeForOneTurn;
		counting = true;
	}

	public void StopTimer()
	{
		if (timerText!=null)
            timerText.text = "";

		if (timerFillImage != null)
            timerFillImage.fillAmount = 1;

		counting = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (counting) 
		{
			timeTillZero -= Time.deltaTime;
            if (timerText!=null)
                timerText.text = ToString();

            fillFraction = timeTillZero / timeForOneTurn;
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = fillFraction;
            }

            // check for TimeExpired
			if(timeTillZero <= 0)
			{
				counting = false;
                //RopeGameObject.SetActive(false);
                TimerExpired.Invoke();
			}
		}
	
	}

	public override string ToString ()
	{
		int inSeconds = Mathf.RoundToInt (timeTillZero);
		string justSeconds = (inSeconds % 60).ToString ();
		if (justSeconds.Length == 1)
			justSeconds = "0" + justSeconds;
		string justMinutes = (inSeconds / 60).ToString ();
		if (justMinutes.Length == 1)
			justMinutes = "0" + justMinutes;

		return string.Format ("{0}:{1}", justMinutes, justSeconds);
	}
}
