using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject playerRef;  // Reference to the player's GameObject

    public bool canSeePlayer;  // Whether the enemy can see the player

    private State currentState;  // Current state of the enemy

    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform player;

    [SerializeField] bool patrolPointSet;

    private Vector3 destPoint;

    [SerializeField] float range = 10;

    [SerializeField] LayerMask Room;

    [SerializeField] LayerMask playerLayer;

    private float lookTimer = 5f;

    private Animator animator;

    [SerializeField] private float sightRange;
    [SerializeField] private float  attackRange;
    public bool inAttackRange;
    
    public bool hidden;

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] float temp1, temp2;

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
        temp1 = sightRange;
        temp2 = attackRange;
    }

    void Update()
    {
        canSeePlayer = Physics.CheckSphere(transform.position,sightRange, playerLayer);
        inAttackRange = Physics.CheckSphere(transform.position, attackRange,playerLayer);

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

        if(lookTimer < 0 ) 
        {
            animator.SetBool("isSearching", true); 
            currentState = State.Patrol; 
        }
        
        if (canSeePlayer)
        {
            animator.SetBool("isChasing", true);  // Stop walking animation
            animator.SetBool("isSearching", false);
            Debug.Log("I see you!");
        }
    }

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

            if (Vector3.Distance(transform.position, destPoint) < 0.5)
            {
                lookTimer = Random.Range(3f, 6f); 
                patrolPointSet = false;
                animator.SetBool("isSearching", false); 
                currentState = State.Idle; 
            }
        }

        if (canSeePlayer && !hidden)
        {
            currentState = State.Chase;  // Transition to Chase state
            animator.SetBool("isSearching", false);
            animator.SetBool("isChasing", true);
            Debug.Log("I see you!");
        }
    }

    void ChasePlayer()
    {
        animator.SetBool("isChasing", true); 
        navAgent.speed = 3f; 
        navAgent.SetDestination(player.position);

        if (!canSeePlayer && currentState != State.Patrol && currentState != State.Idle)
        {
            destPoint = player.position;
            currentState = State.Patrol;
            animator.SetBool("isChasing", false);  
            Debug.Log("Where did you go?");
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

    void enableAttack()
    {
        boxCollider.enabled = true;
    }

    void disableAttack()
    {
        boxCollider.enabled = false;
    }

}
