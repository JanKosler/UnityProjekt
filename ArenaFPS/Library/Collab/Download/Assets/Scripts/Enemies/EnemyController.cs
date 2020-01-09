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
    /// 
    /// </summary>

    private Transform target;
    private NavMeshAgent agent;

    //targeting player
    [SerializeField] [ReadOnly] private float lookDistance;
    [SerializeField] [ReadOnly] private float feelRadius;
    [SerializeField] [ReadOnly] private float hearRadius;

    [SerializeField] [ReadOnly] private bool playerDetected;
    [SerializeField] [ReadOnly] private bool playerAwareness;

    [SerializeField] private float loseThreshold;
    //wandering
    [SerializeField] [ReadOnly] private float wanderRadius;
    [SerializeField] [ReadOnly] private float wanderSpeed;
    [SerializeField] [ReadOnly] private float chaseSpeed;


    private Vector3 wanderPoint;

    

    [SerializeField] private bool isAlive;

    private float loseTimer = 0f;

    public bool gunShot;
    public float hearingDistance;


    public float health;

    private float fov;

    [SerializeField] private EnemyManager manager;

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
        feelRadius = manager.feelRadius;
        hearRadius = manager.hearRadius;
        wanderRadius = manager.wanderingRadius;
        wanderSpeed = manager.wanderingSpeed;
        chaseSpeed = manager.chasingSpeed;
        loseThreshold = manager.loseThreshold;
        health = manager.health;
        fov = manager.fov;
    }
    

    void Update()
    {
        if (isAlive)
        {
            if (playerDetected)
            {
                agent.speed = chaseSpeed;
                agent.SetDestination(target.position);
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
                LookingForTarget();
                Wander();
            }

            
            /*
            if (distance <= lookRadius || (gunShot && distance <= hearingDistance))
            {
                agent.speed = chasingSpeed;
                isWandering = false;
                playerDetected = true;
                looking4Target = false;
            }
            else
            {
                Wander();
                
                playerDetected = false;
                looking4Target = true;
            }*/
        }   
    }
    
    public void LookingForTarget()
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
    }
    public void OnDetection()
    {
        playerDetected = true;
        playerAwareness = true;
        loseTimer = 0f;
    }
    //wandering
    /*
    IEnumerator Wander()
    {
        isWandering = true;
        inCoroutine = true;
        agent.speed = wanderingSpeed;
        yield return new WaitForSeconds(timeForNewPath);
        timeForNewPath = manager.timeForNewPath;
        GetNewPath();
        wanderCoroutine = StartCoroutine(Wander());
        inCoroutine = false;
    }*/
    
    
    public void Wander()
    {
        agent.speed = wanderSpeed;
        if (Vector3.Distance(transform.position, wanderPoint) < 2f)
        {
            wanderPoint = RandomWanderPoint();
        }
        else
        {
            agent.SetDestination(wanderPoint);
        }
    }
    //ukazuje okruh hledani hráče
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
    public Vector3 RandomWanderPoint()
    {
        Vector3 randPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }
    void Die()
    {
        isAlive = false;
        Vector3 deathPos = agent.transform.position; //pozice smrti aby se nedokoncovala rutina
        agent.SetDestination(deathPos);
    }

}
