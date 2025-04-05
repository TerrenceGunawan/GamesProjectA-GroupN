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
    

    private enum State
    {
        Patrol,
        Chase,
        Search,
    }

    void Start()
    {
        // Initialize the player reference and start the field of view routine
        playerRef = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Patrol;

        // Start the field of view routine
        StartCoroutine(FOVRoutine());

        navAgent = GetComponent<NavMeshAgent>(); 
    }

    void Update()
    {
        // State-based behavior
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                ChasePlayer();
                break;

            case State.Search:
                SearchForPlayer();
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

    // Patrol logic (not implemented yet)
    void Patrol()
    {
        if (canSeePlayer)  
        {
            currentState = State.Chase;  // Transition to Chase state
            Debug.Log("I see you!");
        }
    }

    // Chase logic - move the enemy towards the player
    void ChasePlayer()
    {
        // // Move the enemy towards the player at a set speed
        // transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, chaseSpeed * Time.deltaTime);

        // // Make the enemy face the player while chasing (rotate towards player)
        // Vector3 direction = playerRef.transform.position - transform.position;
        // direction.y = 0;
        
        // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        navAgent.SetDestination(player.position);

        
        if (!canSeePlayer && currentState != State.Patrol)  // If the enemy can no longer see the player
        {
            currentState = State.Patrol; 
            Debug.Log("Where did you go?");
        }
    }

    // Search logic (not implemented yet)
    void SearchForPlayer()
    {
        Debug.Log("SEARCHING FOR PLAYER NOT YET IMPLEMENTED");
    }

}
