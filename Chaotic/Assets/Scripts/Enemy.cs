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
    [SerializeField] private Player player;
    [SerializeField] private Flashlight flashlight;

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

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private AudioClip chasingSound;
    private AudioSource audioSource;
    private bool lostPlayer;
    private bool chaseSound;

    public float proximity;

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
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = chasingSound;

        navAgent.updateRotation = true;
        disableAttack();
        StartCoroutine(FOVRoutine());
    }


    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

private void FieldOfViewCheck()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);  // Calculate the distance to the player

    Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, playerLayer);

    if (rangeChecks.Length != 0) 
    {
        Transform target = rangeChecks[0].transform;
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    if (distanceToPlayer < proximity)
    {
        Debug.Log("You are way too close to me right now!");
        canSeePlayer = true;  
    }
}

    void Update()
    {
        if (flashlight.On)
        {
            radius = 15f;
        }
        else
        {
            radius = 11f;
        }

        if (lostPlayer)
        {
            SetNextPatrolPoint();
            lostPlayer = false;
        }

        if (player.IsHidden)
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
        }
    }

    void Patrol()
    {
        navAgent.speed = 2f;
        chaseSound = true;

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
        navAgent.speed = 4f;
              Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToPlayer), 300f * Time.deltaTime);
        navAgent.SetDestination(player.transform.position);
        animator.SetBool("isChasing", true);

        if (!audioSource.isPlaying && chaseSound)
        {
            audioSource.PlayOneShot(chasingSound);
            chaseSound = false;
        }


        if (!canSeePlayer && currentState != State.Patrol && currentState != State.Idle)
        {
            lostPlayer = true;
            destPoint = player.transform.position;
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
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToPlayer), 360f * Time.deltaTime);
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
            TeleportToFurthestPatrolPoint();
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
    public void TeleportToFurthestPatrolPoint()
    {
         float maxDistance = 0f;
        Vector3 furthestPoint = transform.position;
        
        foreach (Transform point in patrolPoints)
        {
            float distance = Vector3.Distance(transform.position, point.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPoint = point.position;
            }
        }
        navAgent.Warp(furthestPoint);
        Debug.Log("Enemy teleported to patrol point");
    }
}
