using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, iDamage, iPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    //[SerializeField] Collider hitBox;
    //[SerializeField] Collider swordCol;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float animSpeed;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootAngle;
    [SerializeField] GameObject bullet;
    [SerializeField] int speed;

    //public spawner whereISpawned;

    [SerializeField] GameObject player;
    Vector3 playerDir;
    Vector3 pushBack;
    bool playerInRange;
    bool isShooting;
    float stoppingDistOrig;
    float angleToplayer;
    bool destinationChoosen;
    Vector3 startingPos;
    float speedOrig;

    void Start()
    {
        // moves enemy
        player = GameObject.FindGameObjectWithTag("Player");

        //use a singleton to control multiple objects in the game
        //see gameManager.cs
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance; 
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            float agentVel = agent.velocity.normalized.magnitude;
            //anim.SetFloat("string name", float) // .magnitude ignores +/- numbers
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentVel, Time.deltaTime + animSpeed));


            if (playerInRange && !canViewPlayer())
            {
                StartCoroutine(roam());
            }
            else if (!playerInRange)
            {
                StartCoroutine(roam());
            }
        }
        //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
    }
    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.05f && !destinationChoosen) 
        {
            destinationChoosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destinationChoosen = false;
        }
    }
    bool canViewPlayer()
    {

        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToplayer = Vector3.Angle(playerDir, transform.forward);
#if(UNITY_EDITOR)
        Debug.Log(angleToplayer);
        Debug.DrawRay(headPos.position, playerDir);
#endif 
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToplayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();

                    if (!isShooting && angleToplayer <= shootAngle)
                    {
                        StartCoroutine(shoot());
                    }
                }
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }
    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    public void takeDamage(int amount)
    {

        HP -= amount;
        //Boss.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            //isDefeated = true;
            agent.enabled = false;
            anim.SetBool("Death", true);
            StopAllCoroutines();
        }
        else
        {

            anim.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            agent.SetDestination(gameManager.instance.player.transform.position);

        }
    }
    IEnumerator stopMoving()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(0.1f);
        agent.speed = speedOrig;
    }
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    void faceTarget()
    {
        //face towards target
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
            agent.stoppingDistance = 0;
        }
    }
    //public void hitBoxOff()
    //{
    //    swordCol.enabled = false;
    //}
    //public void hitBoxOn()
    //{
    //    swordCol.enabled = true;
    //}
    //public void whereISpawned(spawner where)
    //{

    //}
}

