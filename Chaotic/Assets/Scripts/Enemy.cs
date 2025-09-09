using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Player player;    
    [SerializeField] private Transform[] patrolPoints;

    [Header("Behaviour")]
    [SerializeField] private float maxTeleportInterval = 2.5f;
    [SerializeField] private float turnSpeed = 360f;      

    private Transform transform; 
    private Transform playerT;
    private float teleportInterval;
    private int index = -1;
    private float timer = 0f;

    void Awake()
    {
        transform = GetComponent<Transform>();
    }

    void Start()
    {
        playerT = player ? player.transform : GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            index = Random.Range(0, patrolPoints.Length);
            TeleportTo(patrolPoints[index].position);
        }
        teleportInterval = Random.Range(0.5f, maxTeleportInterval);
    }

    void Update()
    {
        if (playerT == null || patrolPoints == null || patrolPoints.Length == 0) return;

        if (player.EnemyVisible)
        {
            FacePlayerSmooth();
        }
        
        timer += Time.deltaTime;
        if (timer >= teleportInterval)
        {
            timer = 0f;
            teleportInterval = Random.Range(0.5f, maxTeleportInterval);

            int next = 0;
            if (patrolPoints.Length > 1)
            {
                do { next = Random.Range(0, patrolPoints.Length); }
                while (next == index);
            }
            index = next;

            TeleportTo(patrolPoints[index].position);
        }
    }

    private void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
    }

    private void FacePlayerSmooth()
    {
        Vector3 dir = playerT.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            var target = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }
}
