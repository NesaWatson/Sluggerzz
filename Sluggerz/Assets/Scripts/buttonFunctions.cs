using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    SurvivalModeManager SurvivalModeManager;
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }
    public void quit()
    {
        Application.Quit();
    }
    public void respawnPlayer()
    {
        gameManager.instance.stateUnpause();
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.playerScript.refillHP();
        gameManager.instance.playerScript.updatePlayerHP();
        SurvivalModeManager.resetTimer();
    }
}

