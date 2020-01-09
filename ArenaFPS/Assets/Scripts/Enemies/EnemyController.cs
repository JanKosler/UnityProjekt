using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// 
    /// 
    /// Logika nepritele
    /// 
    /// musi mit vzdy prednastaven wanderingRadius co neni nula jinak se rozbije
    /// 
    /// 
    /// </summary>

    private Transform target;
    private NavMeshAgent agent;
    private Rigidbody enemyRb;

    private GameObject enemyObj;

    [Header("Targeting")]
    [SerializeField] private float lookDistance = 15f;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private bool playerDetected;
    [SerializeField] private bool playerAwareness;
    private float loseThreshold;
    private float loseTimer = 0f;
    private float fov;

    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 20f; //nesmi byt nula defaultne
    [SerializeField] private float wanderSpeed;
    private Vector3 wanderPoint;

    [Header("Health")]
    [SerializeField] private bool isAlive;
    [SerializeField] private float enemyHealth;

    public enum AIState
    {
        Wander,
        Attack
    }
    [Header("AI State")]
    [SerializeField] private AIState aiState;


    private Material wanderMat;
    private Material attackMat;
    private GameObject[] eyes;


    [SerializeField] private EnemyManager manager;

    [SerializeField] private float attackRadius = 3.5f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackTreshold = 0.8f;
    [SerializeField] private float timeFromLastAttack;

    void Start()
    {
        isAlive = true;
        //nastavujeme hledany objekt, musime udelat bake sceny (NavMesh)
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();

        

        manager = gameObject.GetComponentInParent<EnemyManager>();

        wanderPoint = RandomWanderPoint();

        //inicializace nastvenych hodnot
        lookDistance = manager.lookDistance;
        wanderRadius = manager.wanderRadius;
        wanderSpeed = manager.wanderSpeed;
        chaseSpeed = manager.chaseSpeed;
        loseThreshold = manager.loseThreshold;
        enemyHealth = manager.health;
        fov = manager.fov;
    }

    void OnEnable()
    {
        isAlive = true;
        //nastavujeme hledany objekt, musime udelat bake sceny (NavMesh)
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();

        

        manager = gameObject.GetComponentInParent<EnemyManager>();

        wanderPoint = RandomWanderPoint();

        //inicializace nastvenych hodnot
        lookDistance = manager.lookDistance;
        wanderRadius = manager.wanderRadius;
        wanderSpeed = manager.wanderSpeed;
        chaseSpeed = manager.chaseSpeed;
        loseThreshold = manager.loseThreshold;
        enemyHealth = manager.health;
        fov = manager.fov;
    }

    void Update()
    {
        timeFromLastAttack += Time.deltaTime;
        if (isAlive)
        { 
            if (playerDetected)
            {
                agent.speed = chaseSpeed;
                aiState = AIState.Attack;
                agent.SetDestination(target.position);
                AttackPlayer();
                if (!playerAwareness)
                {
                    loseTimer += Time.deltaTime;
                    if(loseTimer >= loseThreshold)
                    {
                        playerDetected = false;
                        loseTimer = 0;  
                    }
                }
            }
            else
            { 
                agent.speed = wanderSpeed;
                aiState = AIState.Wander;
                Wander();   
            }
            LookingForTarget();
        }
    }
    [SerializeField] private float distanceFromPlayer;
    private void AttackPlayer()
    {
        distanceFromPlayer = Vector3.Distance(target.transform.position, transform.position);
        if(distanceFromPlayer <= attackRadius)
        {
            if(timeFromLastAttack >= attackTreshold)
            {
                EnemyManager.playerHealth -= attackDamage;
                timeFromLastAttack = 0;
            }      
        }    
    }

    private void LookingForTarget()
    {
        if(Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(target.position)) < fov / 2f)
        {
            if(Vector3.Distance(target.position, transform.position) < lookDistance)
            {
                RaycastHit hit;
                if(Physics.Linecast(transform.position, target.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        OnDetection();
                    }
                    else
                    {
                        playerAwareness = false;
                    }
                }
                else
                {
                    playerAwareness = false;
                }
            }
            else
            {
                playerAwareness = false;
            }
        }
        else
        {
            playerAwareness = false;
        }
    }

    public void OnDetection()
    {
        playerDetected = true;
        playerAwareness = true;
        loseTimer = 0f;
    }

    private void Wander()
    {
        if (Vector3.Distance(transform.position, wanderPoint) < 2f)
        {
            wanderPoint = RandomWanderPoint();
        }
        else
        {
            agent.SetDestination(wanderPoint);
        }
    }
    
    private Vector3 RandomWanderPoint()
    {
        Vector3 randPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }
    
    public void TakeDamage(float amount)
    {
        enemyHealth -= amount;
        if (enemyHealth <= 0f && isAlive)
        {
            Die();
        }
    }
    private void Die()
    {
        EnemyManager.AddKill();
        isAlive = false;
        StartCoroutine(DestroyObjectAfterSomeTime(2f));
        agent.isStopped = true;
    }
    private IEnumerator DestroyObjectAfterSomeTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }


    //ukazuje okruh hledani hráče
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    private void ChangeEyeColor()
    {
        eyes = GameObject.FindGameObjectsWithTag("Eye");
        switch (aiState)
        {
            case AIState.Wander:
                foreach (GameObject eye in eyes)
                {
                    eye.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(wanderMat);
                }
                break;
            case AIState.Attack:
                foreach (GameObject eye in eyes)
                {
                    eye.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(attackMat);
                }
                break;
        }
    }
}
