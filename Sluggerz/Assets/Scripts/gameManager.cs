using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public tutorialManager tutorialManager;
    public weaponStats weaponStats;
    public bossEnemy boss;
    public SurvivalModeManager survivalModeManager;

    public GameObject player;
    public playerController playerScript;

    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject playerDamageFlash;
    [SerializeField] GameObject checkPointMenu;

    public GameObject playerSpawnPos;
    public Image playerHPBar;
    public Image playerShieldBar;
    public Image playerStaminaBar;
    public GameObject shieldUI;
    public GameObject staminaUI;


    [SerializeField] public TMP_Text ammoText;

    [SerializeField] TMP_Text bossRemainingText;

    [SerializeField] int bossRemaining;

    private int durationInSecs;

    public bool isPaused;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        boss = FindObjectOfType<bossEnemy>();

        if(boss != null )
        {
            bossRemaining = 1;
            bossRemainingText.text = bossRemaining.ToString("0");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            statePause();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(isPaused);
        activeMenu = null;
    }
    public void updateGameGoal(int amount)
    {

        bossRemaining += amount;

        bossRemainingText.text = bossRemaining.ToString("0");

        if (bossRemaining <= 0)
        {
            StartCoroutine(youWin());
            
        }
    }
    public IEnumerator youWin()
    {
        yield return new WaitForSeconds(3);
        if(SceneManager.GetActiveScene().name == "Survival Mode" && survivalModeManager.timerEnded)
        {
            statePause();
            activeMenu = winMenu;
            activeMenu.SetActive(isPaused);
        }
        if (SceneManager.GetActiveScene().name == "Campaign Mode")
        {
            statePause();
            activeMenu = winMenu;
            activeMenu.SetActive(isPaused);
            Time.timeScale = 1;
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene("Credits");
        }
    }
    public void youLose()
    {
        statePause();
        activeMenu = loseMenu;
        activeMenu.SetActive(isPaused);
    }
    public IEnumerator playerFlashDamage()
    {
        playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageFlash.SetActive(false);
    }
    public void enableShield()
    {
        if(shieldUI != null)
        {
            shieldUI.SetActive(true);
        }
    }
    public void disableShield()
    {
        if(shieldUI != null)
        {
            shieldUI.SetActive(false);
            playerScript.shield = 0;
        }
    }
    public void enableStamina()
    {
        if (staminaUI != null)
        {
            staminaUI.SetActive(true);
        }
    }
    public void disableStamina()
    {
        if (staminaUI != null)
        {
            staminaUI.SetActive(false);
        }
    }
    public IEnumerator checkPointPopup()
    {
        checkPointMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        checkPointMenu.SetActive(false);

    }
    public void SavePlayerState()
    {
        int playerHP = playerScript.GetPlayerHP();
        PlayerPrefs.SetInt("Player_HP", playerHP);

        int gunCount = playerScript.GetPlayerGunsCount();
        PlayerPrefs.SetInt("Guns_Count", gunCount);
        for (int i = 0; i < gunCount; i++)
        {
            WeaponRuntimeData weapon = playerScript.GetPlayerGun(i);
            if (weapon != null)
            {
                PlayerPrefs.SetString($"Gun_Type_{i}", weapon.config.weaponName);
                PlayerPrefs.SetInt($"Gun_Ammo_{i}", weapon.ammoCur);
            }
        }
    }
    public void LoadPlayerState()
    {
        int playerHP = PlayerPrefs.GetInt("Player_HP", 10);
        playerScript.SetPlayerHP(playerHP);

        int gunCount = PlayerPrefs.GetInt("Guns_Count", 0);
        for (int i = 0; i < gunCount; i++)
        {
            string gunName = PlayerPrefs.GetString($"Gun_Type_{i}", "");
            int gunAmmo = PlayerPrefs.GetInt($"Gun_Ammo_{i}", 0);


            playerScript.AddWeapon(gunName, gunAmmo);
        }
    }
    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
