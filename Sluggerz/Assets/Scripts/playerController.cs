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

    private void Start()
    {
        HPOrig = HP;
        spawnPlayer();

    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance);

        movement();
        selectWeapon();

        if (!gameManager.instance.isPaused && weaponList.Count > 0 && Input.GetButton("shoot") && !isShooting)
        {
            StartCoroutine(shoot());
        }
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

        //pushBack = Vector3.zero;
    }
    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            Debug.Log("Hit: " + hit.transform.name);

            iDamage damageable = hit.collider.GetComponent<iDamage>();
            if (damageable != null && hit.transform != transform)
            {
                Debug.Log("Damaging: " + hit.transform.name);
                damageable.takeDamage(shootDamage);
            }
            else
            {
                Debug.Log("No damageable object found.");
            }

        }
        else
        {
            Debug.Log("Raycast didn't hit anything.");
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
    public void giveShield(int amount)
    {
        shield = amount;
        gameManager.instance.enableShield();
    }
    public void giveStamina(int amount)
    {
        StartCoroutine(activateStamina(amount, durationInSecs));
        gameManager.instance.enableStamina();
    }
    IEnumerator activateStamina(float speedMultiplier, float duration)
    {
        float speedOrig = playerSpeed; 
        playerSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);
        playerSpeed = speedOrig;
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
        HP = HPOrig;
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

    }
    public void updatePlayerStamina()
    {
        gameManager.instance.playerStaminaBar.fillAmount = (float)stamina / durationInSecs;

    }
    public void weaponPickup(weaponStats weapon)
    {
        weaponList.Add(weapon);

        shootDamage = weapon.attackDamage;
        shootDistance = weapon.attackDistance;
        shootRate = weapon.attackRate;

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

        shootDamage = weaponList[selectedWeapon].attackDamage;
        shootDistance = weaponList[selectedWeapon].attackDistance;
        shootRate = weaponList[selectedWeapon].attackRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<Renderer>().sharedMaterial = weaponList[selectedWeapon].model.GetComponent<Renderer>().sharedMaterial;
    }
}
