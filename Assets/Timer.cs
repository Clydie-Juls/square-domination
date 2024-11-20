using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeToStartCountdown;
    public int minutes;
    public int countdownBeforeStart;

    public TextMeshProUGUI minutesUI;
    public TextMeshProUGUI secondsUI;

    public GameObject countdownTimer;

    public GameObject timesUpUI;

    [HideInInspector]
    public float currentTimeInSeconds;

    bool startCountdown;
    void Start()
    {
        currentTimeInSeconds = minutes * 60f;
        StartCoroutine(Countdown());
    }

    // Update is called once per frame
    void Update()
    {
        
        if(FindObjectOfType<InputManager>().isTimed)
        {
            int _minutes = (int)(currentTimeInSeconds / 60f);
            int _seconds = (int)(currentTimeInSeconds % 60f); 
            string secondsText = (int)(currentTimeInSeconds % 60f) == 0? "00": _seconds.ToString();
            minutesUI.text = _minutes.ToString() + ":";
            if (_seconds < 10)
            {
                secondsUI.text = "0" + _seconds.ToString();
            }
            else
            {
                secondsUI.text = secondsText;
            }
            currentTimeInSeconds = Mathf.Clamp(currentTimeInSeconds, 0, minutes * 60f);
            if(startCountdown)
            {
                if(currentTimeInSeconds > 0)
                {
                    currentTimeInSeconds -= Time.deltaTime;
                    timesUpUI.gameObject.SetActive(false);
                }

                else
                {
                    timesUpUI.gameObject.SetActive(true);
                    TextMeshProUGUI _text = timesUpUI.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

                    if (FindObjectOfType<SpawnManager>().scores[0] > FindObjectOfType<SpawnManager>().scores[1])
                        _text.text = "Player 1 Wins!";
                     else if (FindObjectOfType<SpawnManager>().scores[1] > FindObjectOfType<SpawnManager>().scores[0])
                        _text.text = "Player 2 Wins!";
                    else
                        _text.text = "Its A Draw";

                    Time.timeScale = 0f;
                }
            }
        }

        else
        {
            minutesUI.gameObject.SetActive(false);
            secondsUI.gameObject.SetActive(false);
        }    
    }

    IEnumerator Countdown()
    {
        Time.timeScale = 0f;
        countdownTimer.SetActive(false);
        yield return new WaitForSecondsRealtime(timeToStartCountdown);
        for (int i = 0; i < countdownBeforeStart; i++)
        {
            countdownTimer.SetActive(true);
            countdownTimer.GetComponent<TextMeshProUGUI>().text = (countdownBeforeStart - i).ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        Time.timeScale = 1f;        
        countdownTimer.GetComponent<TextMeshProUGUI>().text = "GO!";
        countdownTimer.GetComponent<TextMeshProUGUI>().fontSize += 10;
        startCountdown = true;
        yield return new WaitForSecondsRealtime(1f);
        FindObjectOfType<EventManager>().canPause = true;
        countdownTimer.SetActive(false);
    }
}
