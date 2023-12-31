using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, iDamage, iPhysics
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] public int HP;
    [Range(3, 10)][SerializeField] float playerSpeed;
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(1, 3)][SerializeField] int jumpsMax;
    [Range(-35, -15)][SerializeField] float gravityValue;
    [Range(1, 10)][SerializeField] int pushBackResolve;

    [Header("----- Weapon Stats -----")]
    [SerializeField] List<weaponStats> weaponList = new List<weaponStats>();
    [SerializeField] GameObject weaponModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;


    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move; 
    private Vector3 pushBack;
    private int jumpedTimes;
    public bool isShooting;
    public int HPOrig;
    public int shieldOrig;
    int selectedWeapon;
    public int shield;
    public int stamina;
    public int durationInSecs;

    private List<WeaponRuntimeData> playerGuns = new List<WeaponRuntimeData>();

    private void Start()
    {
        HPOrig = HP;
        spawnPlayer();

    }

    void Update()
    {
        movement();
        selectWeapon();

        if (!gameManager.instance.isPaused && weaponList.Count > 0 && Input.GetButton("shoot") && !isShooting)
        {
            StartCoroutine(shoot());
        }
    }
    public void PlayerCheckpointRefresh()
    {
        refillHP();

    }
    void movement()
    {
        if (pushBack.magnitude > 0.01f)
        {
            pushBack.x = Mathf.Lerp(pushBack.x, 0, Time.deltaTime * pushBackResolve);
            pushBack.y = Mathf.Lerp(pushBack.y, 0, Time.deltaTime * pushBackResolve * 3);
            pushBack.z = Mathf.Lerp(pushBack.z, 0, Time.deltaTime * pushBackResolve);
        }

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            jumpedTimes = 0;
            playerVelocity.y = 0f;
        }

        move = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y = jumpHeight;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move((playerVelocity + pushBack) * Time.deltaTime);

    }
    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            iDamage damageable = hit.collider.GetComponent<iDamage>();
            if (damageable != null && hit.transform != transform)
            {
                damageable.takeDamage(shootDamage);
            }
        }
        if (weaponList[selectedWeapon].weaponAudio != null)
        {
            AudioSource.PlayClipAtPoint(weaponList[selectedWeapon].weaponAudio, hit.point);
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void physics(Vector3 dir)
    {
        pushBack += dir;
    }
    public void giveHP(int amount)
    {
        HP += amount;
        updatePlayerHP();
    }
    public void refillHP()
    {
        HP = HPOrig;
        updatePlayerHP();
    }
    public void giveShield(int amount)
    {
        shield = amount;
        shieldOrig = amount;
        gameManager.instance.enableShield();
        updatePlayerShield();
    }
    public void giveStamina(int amount)
    {
        StartCoroutine(activateStamina(amount, durationInSecs));
        gameManager.instance.enableStamina();
        updatePlayerStamina();
    }
    IEnumerator activateStamina(float speedMultiplier, float duration)
    {
        float speedOrig = playerSpeed; 
        playerSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);
        playerSpeed = speedOrig;
        gameManager.instance.disableStamina();
        updatePlayerStamina();
    }
    public void takeDamage(int amount)
    {
        if(shield != 0)
        {
           shield -= amount;
           StartCoroutine(gameManager.instance.playerFlashDamage());
           updatePlayerShield();
        }
        else
        {
            gameManager.instance.disableShield();
            HP -= amount;
            StartCoroutine(gameManager.instance.playerFlashDamage());
            updatePlayerHP();
        }

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void spawnPlayer()
    {
        gameManager.instance.LoadPlayerState();
        SetPlayerHP(HP);
        updatePlayerHP();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
    public void updatePlayerHP()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;

    }
    public void updatePlayerShield()
    {
        gameManager.instance.playerShieldBar.fillAmount = (float)shield / shieldOrig;

        if(shield <= 0)
        {
            gameManager.instance.disableShield();
        }
    }
    public void updatePlayerStamina()
    {
        gameManager.instance.playerStaminaBar.fillAmount = (float)stamina / durationInSecs;

    }
    public void weaponPickup(weaponStats weapon)
    {
        weaponList.Add(weapon);

        shootDamage = weapon.gunDamage;
        shootDistance = weapon.fireDistance;
        shootRate = weapon.shootSpeed;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<Renderer>().sharedMaterial = weapon.model.GetComponent<Renderer>().sharedMaterial;

        selectedWeapon = weaponList.Count - 1;

    }
    void selectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon < weaponList.Count - 1)
        {
            selectedWeapon++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)
        {
            selectedWeapon--;
            changeWeapon();
        }
    }
    void changeWeapon()
    {
        shootDamage = weaponList[selectedWeapon].gunDamage;
        shootDistance = weaponList[selectedWeapon].fireDistance;
        shootRate = weaponList[selectedWeapon].shootSpeed;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<Renderer>().sharedMaterial = weaponList[selectedWeapon].model.GetComponent<Renderer>().sharedMaterial;
    }
    public int GetPlayerHP()
    {
        return HP;
    }
    public void SetPlayerHP(int hp)
    {
        HPOrig = hp;
        hp = HP;
    }
    public WeaponRuntimeData GetPlayerGun(int index)
    {
        if (index >= 0 && index < playerGuns.Count)
        {
            return playerGuns[index];
        }
        return null;
    }

    public weaponStats GetWeaponConfig(string weaponName)
    {
        foreach (weaponStats weapon in weaponList)
        {
            if (weapon.weaponName == weaponName)
            {
                return weapon;
            }
        }
        return null;
    }

    public int GetPlayerGunsCount() { return playerGuns.Count; }

    public void AddWeapon(string gunName, int gunAmmo)
    {
        weaponStats weaponConfig = GetWeaponConfig(gunName);

        if (weaponConfig != null)
        {
            WeaponRuntimeData newWeapon = new WeaponRuntimeData { config = weaponConfig, ammoCur = gunAmmo };
            playerGuns.Add(newWeapon);
        }
    }
}
