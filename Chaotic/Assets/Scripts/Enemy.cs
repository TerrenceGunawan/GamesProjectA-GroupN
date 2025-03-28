using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float detectionRange = 10f;  // The range at which the enemy detects the player
    public float chaseSpeed = 5f;  // Speed at which the enemy chases the player

    private State currentState;

    private enum State
    {
        Patrol,
        Chase,
        Search,
    }

    void Start()
    {
        // Initialize the enemy to start in the Patrol state
        currentState = State.Patrol;
    }

    void Update()
    {
        // Calculate the distance to the player each frame
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Handle the state transitions and actions
        switch (currentState)
        {
            case State.Patrol:
                Patrol(distanceToPlayer);
                break;

            case State.Chase:
                ChasePlayer(distanceToPlayer);
                break;

            case State.Search:
                SearchForPlayer();
                break;
        }
    }

    void Patrol(float distanceToPlayer)
    {
        Debug.Log("Patrolling...");
        
        // Transition to Chase state if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;  // Transition to Chase state
            Debug.Log("I see you!");
        }
    }

    void ChasePlayer(float distanceToPlayer)
    {
        // Move the enemy towards the player at a set speed
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        // Make the enemy face the player while chasing (rotate towards player)
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        // If the player moves out of detection range, return to patrol state
        if (distanceToPlayer >= detectionRange)
        {
            currentState = State.Patrol;  // Transition to Patrol state
            Debug.Log("Where did you go?");
        }
    }

    void SearchForPlayer()
    {
        // Placeholder for search behavior
        Debug.Log("Searching...");
    }
}
