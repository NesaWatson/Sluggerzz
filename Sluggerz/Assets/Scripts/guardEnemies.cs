using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class guardEnemy : MonoBehaviour, iDamage, iPhysics, iAlertable
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent guard;
    [SerializeField] Animator animate;
    [SerializeField] Transform headPos;
    [SerializeField] LayerMask playerLayer;

    [Header("----- Enemy Stats -----")]
    [Range(0, 30)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(45, 180)][SerializeField] int viewDistance;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [SerializeField] float animSpeed;
    [SerializeField] float attackAnimDelay;

    [Header("----- Weapon Stats -----")]
    [SerializeField] GameObject bat;
    [SerializeField] Transform batHand;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int batDamageAmount;
    [SerializeField] int attackAngle;

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
    GameObject currentBat;
    public playerController playerController;
    bool isAlerted;
    

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = guard.stoppingDistance;


        playerTransform = gameManager.instance.player.transform;

    }
    void Update()
    {
        {
            if (guard.isActiveAndEnabled)
            {
                float agentVel = guard.velocity.normalized.magnitude;

                animate.SetFloat("Speed", Mathf.Lerp(animate.GetFloat("Speed"), agentVel, Time.deltaTime * animSpeed));

                if (playerInRange && canViewPlayer())
                {
                    animate.SetTrigger("Run");
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                    if (distanceToPlayer <= attackRange && !isAttacking)
                    {
                        animate.SetTrigger("Attack");
                        StartCoroutine(meleeAttack());
                    }
                }
                else
                {
                    animate.ResetTrigger("Run");
                    StartCoroutine(wander());
                }
            }
        }
    }
    IEnumerator wander()
    {
        if (guard.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            guard.stoppingDistance = 0;
            yield return new WaitForSeconds(wanderTime);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            guard.SetDestination(hit.position);

            wanderDestination = false;
        }
    }
    bool canViewPlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
       
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit, viewDistance, playerLayer))
        {

            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                guard.stoppingDistance = stoppingDistOrig;
                guard.SetDestination(gameManager.instance.player.transform.position);

                if (guard.remainingDistance <= guard.stoppingDistance)
                {
                    faceTarget();

                    if (!isAttacking && angleToPlayer <= attackAngle)
                    {
                        StartCoroutine(meleeAttack());
                    }
                }
                if(!isAlerted)
                {
                    enemyAlertSystem.instance.AlertEnemies
                        (gameManager.instance.player.transform.position);
                    isAlerted = true;
                }
                return true;
            }
        }
        guard.stoppingDistance = 0;
        return false;
    }
    public void Alert(Vector3 playerPos)
    {
        guard.SetDestination(playerPos);
        StartCoroutine(meleeAttack());
    }
    IEnumerator meleeAttack()
    {
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
            guard.enabled = false;
            animate.SetBool("Death", true);
            StopAllCoroutines();
            StartCoroutine(Deadenemy());
        }
        else
        {
            Vector3 playerDirection = gameManager.instance.player.transform.position - transform.position;
            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            guard.SetDestination(gameManager.instance.player.transform.position);
            enemyAlertSystem.instance.AlertEnemies(gameManager.instance.player.transform.position);
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
        guard.velocity += dir / 3;
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
            Destroy(currentBat);
        }
    }
    IEnumerator Deadenemy()
    {

        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}


