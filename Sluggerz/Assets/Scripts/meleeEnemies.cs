using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class meleeEnemies : MonoBehaviour, iDamage, iPhysics, iAlertable
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animate;
    [SerializeField] Transform headPos;
    [SerializeField] LayerMask playerLayer;


    [Header("----- Enemy Stats -----")]
    [Range(0, 15)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(45, 180)][SerializeField] int viewDistance;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [SerializeField] float animSpeed;
    [SerializeField] float attackAnimDelay;


    [Header("----- Weapon Stats -----")]
    [SerializeField] GameObject weapon;
    [SerializeField] Transform weaponHand;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int weaponDamageAmount;
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
    GameObject currentWeapon;
    public playerController playerController;
    //private bool isDefeated = false;

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;


        playerTransform = gameManager.instance.player.transform;

        
    }
    void Update()
    {

        //if (!isDefeated)
        //{
        if (agent.isActiveAndEnabled)
        {
            float agentVel = agent.velocity.normalized.magnitude;

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
        //}
    }
    IEnumerator wander()
    {
        if (agent.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            agent.stoppingDistance = 3;
            yield return new WaitForSeconds(wanderTime);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            agent.SetDestination(hit.position);

            wanderDestination = false;
        }
    }
    bool canViewPlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        //#if (UNITY_EDITOR)
        //        Debug.Log(angleToPlayer);
        //        Debug.DrawRay(headPos.position, playerDir);
        //#endif
        //Debug.DrawRay(headPos.position, playerDir, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit, viewDistance, playerLayer))
        {

            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
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
        agent.stoppingDistance = 0;
        return false;
    }
    public void Alert(Vector3 playerPos)
    {
        agent.SetDestination(playerPos);
        StartCoroutine(meleeAttack());
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
                    player.takeDamage(weaponDamageAmount);
                }
            }
            isAttacking = false;
        }
    }

    //public bool IsDefeated
    //{
    //    get { return isDefeated; }
    //}
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            //isDefeated = true;
            agent.enabled = false;
            animate.SetBool("Death", true);
            StopAllCoroutines();
            StartCoroutine(Deadenemy());
        }
        else
        {
            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            agent.SetDestination(gameManager.instance.player.transform.position);
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
        agent.velocity += dir / 3;
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
            Destroy(currentWeapon);
        }
    }
    IEnumerator Deadenemy()
    {

        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}

