using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void LoadCampaignMode()
   {
        SceneManager.LoadScene("Campaign Mode");
   }
   public void LoadSurvivalMode()
   {
        SceneManager.LoadScene("Survival Mode");
   }
   public void Quitgame()
   {
        Application.Quit();
   }
}
