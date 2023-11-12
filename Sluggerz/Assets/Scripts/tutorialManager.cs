using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class tutorialManager : MonoBehaviour
{
    public Text tutorialMessage;
    public Text tutCompleteMessage;
    public GameObject teleporter;

    private bool enemiesDefeated = false;
    private bool tutorialCompleted = false;

    void Start()
    {
        showTutorialMessage("Pick up the weapon");
    }

    void Update()
    {
        if(enemiesDefeated && !tutorialCompleted)
        {
            showTutorialCompletedMessage();
            showTeleporterPrompt();
        }    
    }
    public void showTutorialMessage(string message)
    {
        tutorialMessage.text = message;

        tutorialMessage.gameObject.SetActive(true); 
    }
    public void hideTutorialMessage(string message)
    {
        tutorialMessage.gameObject.SetActive(false);
    }
    public void showTutorialCompletedMessage()
    {
        tutCompleteMessage.text = "Tutorial Completed";

        tutCompleteMessage.gameObject.SetActive(true);
        tutorialCompleted = true;
    }
    public void showTeleporterPrompt()
    {
        teleporter.SetActive(true);
    }
    public void startMainMenu()
    {
        SceneManager.LoadScene("Campaign Mode");
    }
    public void exitToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
