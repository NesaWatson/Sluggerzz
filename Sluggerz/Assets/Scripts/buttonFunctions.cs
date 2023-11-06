using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
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
    //public void givePlayerHP(int amount)
    //{
    //    gameManager.instance.playerScript.giveHP(amount);
    //}
    public void respawnPlayer()
    {
        gameManager.instance.stateUnpause();
        gameManager.instance.playerScript.spawnPlayer();
    }
    //public void tryAgain()
    //{
    //    gameManager.instance.LoadPlayerState();
    //    gameManager.instance.respawnPlayer();
    //    gameManager.instance.stateUnpause();
    //}
}

