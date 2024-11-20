using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public float timeToEnterNewScene;

    public Animator anim;
    public Animator displayAnim;
    public GameObject PauseBackground;

    public GameObject playDisplay;
    public GameObject settingsDisplay;
    public GameObject instructionsDisplay;

    [HideInInspector]
    public bool canPause;

    bool pause;

    public void EnterNewScene(string scene)
    {
        StartCoroutine(EnteringScene(scene));
    }

    public void Restart()
    {
        StartCoroutine(Restarting());
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowDiplay(int methodNo)
    {
        displayAnim.SetBool("Show",true);
        ShowRespectiveDiplay(methodNo);
    }

    void ShowRespectiveDiplay(int i)
    {
        if(i == 0)
        {
            playDisplay.SetActive(true);
            settingsDisplay.SetActive(false);
            instructionsDisplay.SetActive(false);
        }

        else if (i == 1)
        {
            playDisplay.SetActive(false);
            settingsDisplay.SetActive(true);
            instructionsDisplay.SetActive(false);
        }

        else if (i == 2)
        {
            playDisplay.SetActive(false);
            settingsDisplay.SetActive(false);
            instructionsDisplay.SetActive(true);
        }
    }

    public void EnterGame(bool isTimed)
    {
        StartCoroutine(EnteringScene("Main Game"));
        FindObjectOfType<InputManager>().isTimed = isTimed;
    }

    public void HideDiplay()
    {
        displayAnim.SetBool("Show", false);
    }

    IEnumerator EnteringScene(string scene)
    {
        anim.SetBool("isFadeIn", true);
        yield return new WaitForSecondsRealtime(timeToEnterNewScene);
        SceneManager.LoadScene(scene);
    }

    public void UnPause()
    {
        pause = false;
    }

    IEnumerator Restarting()
    {
        anim.SetBool("isFadeIn", true);
        yield return new WaitForSecondsRealtime(timeToEnterNewScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            if(Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<Timer>().currentTimeInSeconds > 0)
            {
                pause = !pause;
            }
            PauseBackground.SetActive(pause);          
        }

        if(SceneManager.GetActiveScene().buildIndex == 0)
            Time.timeScale = 1f;



        if (canPause)
        {
            if (pause)
            {
                Time.timeScale = 0f;
            }

            else
            {
                Time.timeScale = 1f;
            }
        }
    }
   
}
