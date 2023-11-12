using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalModeManager : MonoBehaviour
{
   public void GameOver()
   {
        SceneManager.LoadScene("MainMenu");
   }
}
