using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SurvivalModeManager : MonoBehaviour
{
   public float survivalTime;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Text timerText;

   private float currentTime;

    private void Start()
    {
        currentTime = survivalTime;
        StartCoroutine(countdown());
    }
    private void Update()
    {
        updateTimerUI();
    }
    IEnumerator countdown()
    {
        while(currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime--; 

            if(currentTime <=0)
            {
                EndGame(true); 
            }
        }
    }
    private void updateTimerUI()
    {
        if(timerText != null)
        {
            timerText.text = FormatTime(currentTime);

        }
        string FormatTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void GameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void EndGame(bool playerWins)
    {
        if (playerWins)
        {
            StopAllCoroutines();
            if (winMenu != null)
            {
                winMenu.SetActive(true);
            }
        }
        else
        {
            StopAllCoroutines();
            if (loseMenu != null)
            {
                loseMenu.SetActive(true);
            }
        }
    }
}
