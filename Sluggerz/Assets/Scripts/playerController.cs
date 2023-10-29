using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, iDamage, iPhysics
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(3, 10)][SerializeField] float playerSpeed;
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(1, 3)][SerializeField] int jumpsMax;
    [Range(-35, -15)][SerializeField] float gravityValue;
    //[Range(1, 10)][SerializeField] int pushBackResolve;

    [Header("----- Weapon Stats -----")]
    [SerializeField] List<weaponStats> weaponList = new List<weaponStats>();
    [SerializeField] GameObject weaponModel;
    [SerializeField] float attackRate;
    [SerializeField] int attackDamage;
    [SerializeField] int attackDistance;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move; 
   // private Vector3 pushBack;
    private int jumpedTimes;
    private bool isAttacking;
    int HPOrig;
    int selectedWeapon;

    private void Start()
    {
        HPOrig = HP;
        spawnPlayer();
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * attackDistance);

        movement();
        selectWeapon();

        if (!gameManager.instance.isPaused && Input.GetButton("attack") && !isAttacking)
        {
            StartCoroutine(attack());
        }
    }

    void movement()
    {
        //if (pushBack.magnitude > 0.01f)
        //{
        //    pushBack.x = Mathf.Lerp(pushBack.x, 0, Time.deltaTime * pushBackResolve);
        //    pushBack.y = Mathf.Lerp(pushBack.y, 0, Time.deltaTime * pushBackResolve * 3);
        //    pushBack.z = Mathf.Lerp(pushBack.z, 0, Time.deltaTime * pushBackResolve);
        //}

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            jumpedTimes = 0;
            playerVelocity.y = 0f;
        }

        move = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y = jumpHeight;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        //controller.Move((playerVelocity + pushBack) * Time.deltaTime);

        //pushBack = Vector3.zero;
    }
    IEnumerator attack()
    {
        if (selectedWeapon >= 0 && selectedWeapon < weaponList.Count)
        {


            if (weaponList[selectedWeapon].ammoCurr > 0)
            {
                isAttacking = true;
                weaponList[selectedWeapon].ammoCurr--;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, attackDistance))
                {
                    //Instantiate(wall, hit.point, transform.rotation);
                    iDamage damageable = hit.collider.GetComponent<iDamage>();
                    Instantiate(weaponList[selectedWeapon].hitEffect, hit.point, weaponList[selectedWeapon].hitEffect.transform.rotation);

                    if (damageable != null)
                    {
                        damageable.takeDamage(attackDamage);
                    }

                }
            }
        }
        else
        {
            Debug.LogWarning("Selected weapon index is out of bounds.");
        }

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }
    public void physics(Vector3 dir)
    {
        //pushBack += dir;
    }
    public void giveHP(int amount)
    {
        HP += amount;
        //then add function in button function
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(gameManager.instance.playerFlashDamage());
        updatePlayerUI();

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void spawnPlayer()
    {
        HP = HPOrig;
        updatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
    public void weaponPickup(weaponStats weapon)
    {
        weaponList.Add(weapon);

        attackDamage = weapon.attackDamage;
        attackDistance = weapon.attackDistance;
        attackRate = weapon.attackRate;

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

        attackDamage = weaponList[selectedWeapon].attackDamage;
        attackDistance = weaponList[selectedWeapon].attackDistance;
        attackRate = weaponList[selectedWeapon].attackRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<Renderer>().sharedMaterial = weaponList[selectedWeapon].model.GetComponent<Renderer>().sharedMaterial;
    }
}
