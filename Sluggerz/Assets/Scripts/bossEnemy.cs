using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossEnemy : MonoBehaviour, iDamage, iPhysics
{
    private gameManager gameManager;

    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent boss;
    [SerializeField] Animator animate;
    [SerializeField] Transform headPos;
    [SerializeField] LayerMask playerLayer;

    [Header("----- Enemy Stats -----")]
    [Range(0, 50)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [Range(0, 30)][SerializeField] float teleportDist;
    [Range(0, 10)][SerializeField] float teleportCooldown;
    [Range(1, 3)][SerializeField] float animSpeed;
    [SerializeField] float attackAnimDelay;

    [Header("----- Weapon Stats -----")]
    [SerializeField] GameObject bat;
    [SerializeField] Transform batHand;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int batDamageAmount;
    [SerializeField] int attackAngle;

    Vector3 playerDir;
    Vector3 playerPos;
    Vector3 pushBack;
    bool playerInRange;
    bool isAttacking;
    float stoppingDistOrig;
    float angleToPlayer;
    bool wanderDestination;

    Vector3 startingPos;
    Transform playerTransform;
    float origSpeed;
    bool canSeePlayer;
    float lastTeleportTime;

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = boss.stoppingDistance;

        playerTransform = gameManager.instance.player.transform;
        gameManager = gameManager.instance;

    }
    void Update()
    {

        if (boss.isActiveAndEnabled)
        {
            if (Time.time - lastTeleportTime >= teleportCooldown)
            {
                float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distToPlayer < teleportDist)
                {
                    teleport(playerTransform.position);
                }
            }
            faceTarget();
            float agentVel = boss.velocity.normalized.magnitude;

            animate.SetFloat("Speed", Mathf.Lerp(animate.GetFloat("Speed"), agentVel, Time.deltaTime + animSpeed));

            if (playerInRange && !canViewPlayer())
            {
                StartCoroutine(wander());
            }
            else if (!playerInRange)
            {
                StartCoroutine(wander());
            }
        }
    }
    void teleport(Vector3 teleportPos)
    {
        Vector3 dirToPlayer = (teleportPos - transform.position).normalized;
        Vector3 offset = dirToPlayer * 1.0f;
        Vector3 finalTeleportPos = teleportPos + offset;

        RaycastHit hitInfo;
        NavMeshHit navMeshHit;

        if (Physics.Raycast(finalTeleportPos, Vector3.down, out hitInfo, 2.0f, LayerMask.GetMask("Ground")))
        {
            finalTeleportPos = hitInfo.point;
        }
        else
        {
            if (NavMesh.SamplePosition(finalTeleportPos, out navMeshHit, teleportDist, 1))
            {
                finalTeleportPos = navMeshHit.position;
            }
            else
            {
                finalTeleportPos = transform.position;
            }
        }
        if (NavMesh.SamplePosition(finalTeleportPos, out navMeshHit, teleportDist, 1))
        {
            boss.Warp(navMeshHit.position);
            lastTeleportTime = Time.time;
        }

    }
    IEnumerator wander()
    {
        if (boss.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            boss.stoppingDistance = 0;
            yield return new WaitForSeconds(wanderTime);

            boss.SetDestination(startingPos);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            boss.SetDestination(hit.position);

            wanderDestination = false;
        }
    }
    bool canViewPlayer()
    {
        boss.stoppingDistance = stoppingDistOrig;
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        //#if(UNITY_EDITOR)
        //        Debug.Log(angleToPlayer);
        //        Debug.DrawRay(headPos.position, playerDir);
        //#endif
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                boss.stoppingDistance = stoppingDistOrig;

                boss.SetDestination(gameManager.instance.player.transform.position);

                if (boss.remainingDistance <= boss.stoppingDistance)
                {
                    faceTarget();

                    if (!isAttacking && angleToPlayer <= attackAngle)
                    {
                        StartCoroutine(meleeAttack());
                    }
                }
                return true;
            }
        }
        boss.stoppingDistance = 0;
        return false;
    }
    IEnumerator meleeAttack()
    {
        //if (isDefeated) yield break;
        if (!isAttacking)
        {
            isAttacking = true;
            animate.SetTrigger("Attack");

            yield return new WaitForSeconds(attackAnimDelay);

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                playerController player = playerTransform.GetComponent<playerController>();

                if (player != null)
                {
                    player.takeDamage(batDamageAmount);
                }
            }
            isAttacking = false;
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;


        if (HP <= 0)
        {
            boss.enabled = false;
            animate.SetBool("Death", true);
            gameManager.instance.updateGameGoal(-1);
            StopAllCoroutines();
            StartCoroutine(Deadenemy());
            gameManager.instance.youWin();
        }
        else
        {
            Vector3 playerDirection = gameManager.instance.player.transform.position - transform.position;
            Quaternion newRotation = Quaternion.LookRotation(playerDirection);
            transform.rotation = newRotation;
            boss.SetDestination(gameManager.instance.player.transform.position);

            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());

        }

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
        boss.velocity += dir / 3;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            boss.stoppingDistance = 0;
        }
    }
    public IEnumerator Deadenemy()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

}


