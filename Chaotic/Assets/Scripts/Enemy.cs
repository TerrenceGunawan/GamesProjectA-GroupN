using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public float radius;  // The radius of the enemy's field of view
    [Range(0, 360)]
    public float angle;  // The angle of the field of view
    // References and variables for patrol points and player detection
    public GameObject playerRef;  // Reference to the player's GameObject
    public bool canSeePlayer;  

    private State currentState;  // Current state of the enemy

    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform player;

    public Vector3 destPoint;
    private bool patrolPointSet = false;

    [SerializeField] private Transform[] patrolPoints;  // Array of patrol points
    private int currentPatrolIndex = 0;  // To keep track of which patrol point to go to next
    [SerializeField] LayerMask playerLayer;  // To detect the player

     [SerializeField] LayerMask obstructionMask; 

    private float lookTimer = 5f;  // Timer for idle states
    private Animator animator;
    [SerializeField] private float attackRange;
    public bool inAttackRange;

    public bool hidden;  // To check if player is hidden (in a hiding spot)

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private AudioClip chasingSound; 
    private AudioSource audioSource;
    private bool lostPlayer;

    private enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
    }  

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Patrol;
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider>();

        navAgent.updateRotation = true; 
        disableAttack();
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck(); 
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, playerLayer);

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

    void Update()
    {

        if (lostPlayer)
        {
            SetNextPatrolPoint();
            lostPlayer = false;
        }
        
        if (hidden)
        {
            currentState = State.Patrol;  // If the player is hidden, go back to patrolling
            animator.SetBool("isChasing", false);  // Stop chasing animation
        }

        inAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

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

            case State.Attack:
                Attack();
                break;
        }
    }

    void Idle()
    {
        animator.SetBool("isSearching", false);
        lookTimer -= Time.deltaTime;

        if (lookTimer < 0)
        {
            animator.SetBool("isSearching", true);
            currentState = State.Patrol;  // Transition to Patrol state
        }

        if (canSeePlayer)
        {
            currentState = State.Chase;
            animator.SetBool("isChasing", true);
            animator.SetBool("isSearching", false);
            Debug.Log("I see you!");
        }
    }

    void Patrol()
    {
        navAgent.speed = 1f;
        
        if (!patrolPointSet)
        {
            SetNextPatrolPoint();
        }

        if (patrolPointSet)
        {
            navAgent.SetDestination(destPoint);
            animator.SetBool("isSearching", true);

            if (Vector3.Distance(transform.position, destPoint) < 1f)
            {
                Debug.Log("test");
                lookTimer = Random.Range(3f, 6f);
                SetNextPatrolPoint();
                animator.SetBool("isSearching", false);
                currentState = State.Idle;  
            }
        }

        if (canSeePlayer)
        {
            currentState = State.Chase;
            animator.SetBool("isSearching", false);
            animator.SetBool("isChasing", true);
            Debug.Log("I see you!");
        }
    }

    void SetNextPatrolPoint()
    {
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        destPoint = patrolPoints[currentPatrolIndex].position;
        patrolPointSet = true;
    }

    void ChasePlayer()
    {
        navAgent.speed = 2f;
        navAgent.SetDestination(player.position);
        animator.SetBool("isChasing", true);

        if (!canSeePlayer && currentState != State.Patrol && currentState != State.Idle)
        {
            lostPlayer = true;
            destPoint = player.position;
            currentState = State.Patrol;
            animator.SetBool("isChasing", false);
            Debug.Log("Lost sight of the player.");
 
        }

        if (canSeePlayer && inAttackRange)
        {
            currentState = State.Attack;
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    void Attack()
    {
        animator.SetBool("isAttacking", true);
        enableAttack();
        navAgent.SetDestination(transform.position);

        if (!inAttackRange)
        {
            currentState = State.Chase;
            animator.SetBool("isAttacking", false);
            disableAttack();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // currentState = State.Chase;  // This can be used if you want to immediately chase when colliding
        }
    }

    void enableAttack()
    {
        boxCollider.enabled = true;  // Enable the attack collider
    }

    void disableAttack()
    {
        boxCollider.enabled = false;  // Disable the attack collider
    }
}
