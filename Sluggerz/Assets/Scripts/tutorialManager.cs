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
    public weaponPickup weaponPickup;

    private int currentStep;
    bool tutorialCompleted;
    public int enemiesRemaining;
    [SerializeField] public TMP_Text enemiesRemainingText;

    void Start()
    {
        currentStep = 1;
        UpdateTutorialSteps(currentStep);
    }

    void Update()
    {
        if (!tutorialCompleted)
        {
            if (currentStep == 1)
            {
                ShowTutorialMessage("Step 1: Pick up the weapon");
                if(weaponPickup != null)
                {
                    currentStep++;
                }
            }
            else if (currentStep == 2)
            {
                ShowTutorialMessage("Step 2: Defeat the enemies");
                if(enemiesDefeated())
                {
                    currentStep++;
                }
            }
            else if (currentStep == 3)
            {
                ShowTutorialCompletedMessage();
                ShowTeleporterPrompt();
                ShowTutorialMessage("\nFind the teleporter to continue");
            }
            
        }
    }
    public void UpdateTutorialSteps(int newStep)
    {
        currentStep = newStep;
    }
    bool enemiesDefeated()
    {
        enemiesRemaining = 0;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if(enemy.activeSelf)
            {
                enemiesRemaining++;
            }
        }
        enemiesRemainingText.text = enemiesRemaining.ToString("0");

        return enemiesRemaining <= 0;
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
