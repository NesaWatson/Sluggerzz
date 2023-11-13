using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class tutorialManager : MonoBehaviour
{
    public Text tutorialMessage;
    public Text tutCompleteMessage;
    public GameObject teleporter;
    public playerController player;
    public int amount;

    private int currentStep = 1;
    bool tutorialCompleted;
    public int enemiesRemaining;
    [SerializeField] public TMP_Text enemiesRemainingText;



    void Start()
    {
        ShowTutorialMessage("Step 1: Pick up the weapon");
    }

    void Update()
    {
        if (enemiesDefeated() && !tutorialCompleted)
        {
            if (currentStep == 1)
            {
                ShowTutorialCompletedMessage();
                ShowTeleporterPrompt();
                ShowTutorialMessage("Step 2: Defeat the enemies");
                currentStep++;
            }
            else if (currentStep == 2)
            {
                ShowTutorialCompletedMessage();
                ShowTeleporterPrompt();
                ShowTutorialMessage("Tutorial complete!\nFind the teleporter to continue");
                currentStep++;
            }
        }
    }

    bool enemiesDefeated()
    {
        enemiesRemaining += amount;

        enemiesRemainingText.text = enemiesRemaining.ToString("0");

        if (enemiesRemaining <= 0)
        {
            ShowTutorialCompletedMessage();
            return true;
        }
        return false;
    }

    public void ShowTutorialMessage(string message)
    {
        tutorialMessage.text = message;
        tutorialMessage.gameObject.SetActive(true);
    }

    public void HideTutorialMessage()
    {
        tutorialMessage.gameObject.SetActive(false);
    }

    public void ShowTutorialCompletedMessage()
    {
        tutCompleteMessage.text = "Tutorial Completed";
        tutCompleteMessage.gameObject.SetActive(true);
        tutorialCompleted = true;
    }

    public void ShowTeleporterPrompt()
    {
        teleporter.SetActive(true);
    }

    public void StartMainGame()
    {
        SceneManager.LoadScene("Campaign Mode");
    }
}
