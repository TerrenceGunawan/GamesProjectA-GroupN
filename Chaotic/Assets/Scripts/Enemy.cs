using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Field of View variables
    public float radius;  // The radius of the enemy's field of view
    [Range(0, 360)]
    public float angle;  // The angle of the field of view

    public GameObject playerRef;  // Reference to the player's GameObject

    public LayerMask targetMask;  // Layer mask for targets (player)
    public LayerMask obstructionMask;  // Layer mask for obstacles (walls)

    public bool canSeePlayer;  // Whether the enemy can see the player

    // Enemy state variables
    public float chaseSpeed = 5f;  // Speed at which the enemy chases the player
    private State currentState;  // Current state of the enemy

    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform player;

    [SerializeField] bool patrolPointSet;

    private Vector3 destPoint;

    [SerializeField] float range = 10;

    [SerializeField] LayerMask Room;

    private float lookTimer = 5f;

    // Animator reference
    private Animator animator;

    private enum State
    {
        Idle,
        Patrol,
        Chase,
    }

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Patrol;
        StartCoroutine(FOVRoutine());
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        // State-based behavior
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                ChasePlayer();
                break;
        }
    }

    // Field of view check - checks if the player is within the enemy's line of sight
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();  // Perform field of view check every 0.2 seconds
        }
    }

    // Check if the player is within the field of view and line of sight
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }
    void Idle()
    {   
        animator.SetBool("isSearching", false);  
        lookTimer -= Time.deltaTime;

        if(lookTimer < 0 ) 
        {
            animator.SetBool("isSearching", true); 
            currentState = State.Patrol; 
        }
        
        if (canSeePlayer)
        {
            currentState = State.Chase;  // Transition to Chase state
            animator.SetBool("isChasing", true);  // Stop walking animation
            animator.SetBool("isSearching", false);
            Debug.Log("I see you!");
        }
    }

    // Patrol logic: enemy will move to random points within a defined radius
    void Patrol()
    {
        if (!patrolPointSet)
        {
            SearchForDest();
        }
        if (patrolPointSet)
        {
            navAgent.speed = 2f; 
            navAgent.SetDestination(destPoint);
            animator.SetBool("isSearching", true);  // Set the walking animation

            if (Vector3.Distance(transform.position, destPoint) < 0.2)
            {
                lookTimer = Random.Range(3f, 6f); 
                patrolPointSet = false;
                animator.SetBool("isSearching", false); 
                currentState = State.Idle; 
            }
        }

        if (canSeePlayer)
        {
            currentState = State.Chase;  // Transition to Chase state
            animator.SetBool("isSearching", false);
            animator.SetBool("isChasing", true);
            Debug.Log("I see you!");
        }
    }

    void ChasePlayer()
    {
        navAgent.speed = 3f; 
        navAgent.SetDestination(player.position);

        if (!canSeePlayer && currentState != State.Patrol && currentState != State.Idle)
        {
            destPoint = player.position;
            currentState = State.Patrol;
            animator.SetBool("isChasing", false);  
            Debug.Log("Where did you go?");
        }

        if (Vector3.Distance(transform.position,player.position) < 5)
        {
            animator.SetBool("isAttacking", true); 
        }
        else 
        {
            animator.SetBool("isAttacking", false); 
        }


    }


    void SearchForDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, Room))
        {
            patrolPointSet = true;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentState = State.Chase;
        }
    }
}
