using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void LoadGame()
   {
        SceneManager.LoadScene("Tutorial Scene");
   }
   public void LoadSurvivalMode()
   {
        SceneManager.LoadScene("Survival Mode");
   }
   public void Quitgame()
   {
        Application.Quit();
   }
    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
