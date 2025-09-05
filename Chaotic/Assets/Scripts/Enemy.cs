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
    public bool CanSeePlayer;

    private State currentState;  // Current state of the enemy

    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
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
    [SerializeField] private AudioClip walkingSound;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip attackSound;
    private AudioSource walkingAudioSource;
    private AudioSource chasingAudioSource;
    private AudioSource runningAudioSource;
    private AudioSource attackAudioSource;
    private bool lostPlayer;
    private bool chaseSound;
    private bool attackSoundPlayed = false;

    private float distanceToPlayer = 0f;
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

        chasingAudioSource = gameObject.AddComponent<AudioSource>();
        chasingAudioSource.clip = chasingSound;
        chasingAudioSource.loop = false;
        chasingAudioSource.playOnAwake = false;

        runningAudioSource = gameObject.AddComponent<AudioSource>();
        runningAudioSource.clip = runningSound;
        runningAudioSource.loop = true;
        runningAudioSource.playOnAwake = false;
        runningAudioSource.pitch = 1f;
        runningAudioSource.volume = 0f;
        runningAudioSource.Play();

        walkingAudioSource = gameObject.AddComponent<AudioSource>();
        walkingAudioSource.clip = walkingSound;
        walkingAudioSource.loop = true;
        walkingAudioSource.playOnAwake = false;
        walkingAudioSource.pitch = 0.7f;
        walkingAudioSource.volume = 0f;
        walkingAudioSource.Play();

        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.clip = attackSound;
        attackAudioSource.loop = false;
        attackAudioSource.playOnAwake = false;

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
                    CanSeePlayer = true;
                }
                else
                {
                    CanSeePlayer = false;
                }
            }
            else
            {
                CanSeePlayer = false;
            }
        }

        if (distanceToPlayer < proximity)
        {
            CanSeePlayer = true;
        }
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (flashlight.On)
            radius = 15f;
        else
            radius = 11f;

        float maxRunningSoundDistance = 17f;
        float maxRunningSoundVolume = 1f;
        float volume = Mathf.Clamp01(1 - (distanceToPlayer / maxRunningSoundDistance));
        runningAudioSource.volume = volume * maxRunningSoundVolume;
        walkingAudioSource.volume = volume * maxRunningSoundVolume;

        if (lostPlayer)
        {
            SetNextPatrolPoint();
            lostPlayer = false;
        }

        // if (player.IsHidden)
        // {
        //     currentState = State.Patrol;
        //     animator.SetBool("isChasing", false);
        // }

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

        if (runningAudioSource.isPlaying)
            runningAudioSource.Pause();
        if (walkingAudioSource.isPlaying)
            walkingAudioSource.Pause();

        if (lookTimer < 0)
        {
            animator.SetBool("isSearching", true);
            currentState = State.Patrol;

            if (!runningAudioSource.isPlaying)
                runningAudioSource.UnPause();
            if (!walkingAudioSource.isPlaying)
                walkingAudioSource.UnPause();
        }

        if (CanSeePlayer)
        {
            currentState = State.Chase;
            animator.SetBool("isChasing", true);
            animator.SetBool("isSearching", false);
        }
    }

    void Patrol()
    {
        navAgent.speed = patrolSpeed;
        chaseSound = true;

        // Play walking audio only, stop running and chasing
        walkingAudioSource.pitch = 0.7f;
        if (!walkingAudioSource.isPlaying)
            walkingAudioSource.Play();

        if (runningAudioSource.isPlaying)
            runningAudioSource.Pause();
        if (chasingAudioSource.isPlaying)
            chasingAudioSource.Stop();

        if (!patrolPointSet)
            SetNextPatrolPoint();

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

        if (CanSeePlayer)
        {
            currentState = State.Chase;
            animator.SetBool("isSearching", false);
            animator.SetBool("isChasing", true);
        }
    }

    void ChasePlayer()
    {
        navAgent.speed = chaseSpeed;

        // Play running audio only, stop walking
        runningAudioSource.pitch = 1f;
        if (!runningAudioSource.isPlaying)
            runningAudioSource.Play();

        if (walkingAudioSource.isPlaying)
            walkingAudioSource.Pause();

        // Play chasing sound once at start of chase
        if (!chasingAudioSource.isPlaying && chaseSound)
        {
            chasingAudioSource.PlayOneShot(chasingSound);
            chaseSound = false;
        }

        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToPlayer), 300f * Time.deltaTime);
        navAgent.SetDestination(player.transform.position);
        animator.SetBool("isChasing", true);

        if (!CanSeePlayer && currentState != State.Patrol && currentState != State.Idle)
        {
            lostPlayer = true;
            destPoint = player.transform.position;
            currentState = State.Patrol;
            animator.SetBool("isChasing", false);
            chaseSound = true; // reset chase sound for next time
        }

        if (CanSeePlayer && inAttackRange)
        {
            currentState = State.Attack;
            attackSoundPlayed = false;
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

        if (!attackSoundPlayed && !attackAudioSource.isPlaying)
        {
        attackAudioSource.PlayOneShot(attackSound);
        attackSoundPlayed = true;
        }

        if (!inAttackRange)
        {
            currentState = State.Chase;
            animator.SetBool("isAttacking", false);
            disableAttack();
        }
    }

        void SetNextPatrolPoint()
    {
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        destPoint = patrolPoints[currentPatrolIndex].position;
        patrolPointSet = true;
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
        CanSeePlayer = false;
        float maxDistance = 0f;
        Vector3 furthestPoint = transform.position;
        foreach (Transform point in patrolPoints)
        {
            float distance = Vector3.Distance(player.transform.position, point.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPoint = point.position;
            }
        }
        navAgent.Warp(furthestPoint);
    }
}
