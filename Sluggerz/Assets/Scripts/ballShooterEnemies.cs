using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ballShooterEnemies : MonoBehaviour, iDamage, iPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent ballShooter;
    [SerializeField] Animator animate;
    [SerializeField] Transform attackPos;
    [SerializeField] Transform headPos;
    [SerializeField] LayerMask playerLayer;

    [Header("----- Enemy Stats -----")]
    [Range(0, 20)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [SerializeField] float animSpeed;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float attackRate;
    [SerializeField] int attackAngle;
    [SerializeField] GameObject baseball;

    Vector3 playerDir;
    Vector3 pushBack;
    bool playerInRange;
    bool isAttacking;
    float stoppingDistOrig;
    float angleToPlayer;
    bool wanderDestination;
    Vector3 startingPos;
    Transform playerTransform;
    float origSpeed;

    //private bool isDefeated = false;

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = ballShooter.stoppingDistance;


        playerTransform = gameManager.instance.player.transform;

        gameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        if (ballShooter.isActiveAndEnabled)
        {
            float agentVel = ballShooter.velocity.normalized.magnitude;

            animate.SetFloat("Speed", Mathf.Lerp(animate.GetFloat("Speed"), agentVel, Time.deltaTime + animSpeed));

            if (playerInRange && canViewPlayer())
            {
                StartCoroutine(attack());
            }
            else
            {
                StartCoroutine(wander());
            }
        }
    }
    IEnumerator wander()
    {
        if (ballShooter.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            ballShooter.stoppingDistance = 0;
            yield return new WaitForSeconds(wanderTime);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            ballShooter.SetDestination(hit.position);

            wanderDestination = false;
        }
    }
    bool canViewPlayer()
    {
        //Boss.stoppingDistance = stoppingDistOrig;
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        //#if (UNITY_EDITOR)
        //        Debug.Log(angleToPlayer);
        //        Debug.DrawRay(headPos.position, playerDir);
        //#endif
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                ballShooter.stoppingDistance = stoppingDistOrig;
                ballShooter.SetDestination(gameManager.instance.player.transform.position);

                if (ballShooter.remainingDistance <= ballShooter.stoppingDistance)
                {
                    faceTarget();

                    if (!isAttacking && angleToPlayer <= attackAngle)
                    {
                        StartCoroutine(attack());
                    }
                }
                return true;
            }
        }
        ballShooter.stoppingDistance = 0;
        return false;
    }
    public void Alert(Vector3 playerPos)
    {
        ballShooter.SetDestination(playerPos);
        StartCoroutine(attack());
    }
    IEnumerator attack()
    {
        while (playerInRange)
        {
            isAttacking = true;
            animate.SetTrigger("Attack");
            yield return new WaitForSeconds(attackRate);
        }
        isAttacking = false;
    }

    //public bool IsDefeated
    //{ get { return isDefeated; } }
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            ballShooter.enabled = false;
            animate.SetBool("Death", true);
            StopAllCoroutines();
            StartCoroutine(Deadenemy());
        }
        else
        {
            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            ballShooter.SetDestination(gameManager.instance.player.transform.position);
        }
    }
    IEnumerator stopMoving()
    {
        ballShooter.speed = 0;
        yield return new WaitForSeconds(0.1f);
        ballShooter.speed = origSpeed;
    }
    public void createBaseball()
    {
        Instantiate(baseball, attackPos.position, transform.rotation);
    }
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }
    public void physics(Vector3 dir)
    {
        ballShooter.velocity += dir / 3;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")
            )
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    IEnumerator Deadenemy()
    {

        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}

