using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Player player;    
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent navAgent; 

    [Header("Behaviour")]
    [SerializeField] private float teleportInterval = 2.5f;
    [SerializeField] private float turnSpeed = 360f;       

    private Transform playerT;
    private int index = -1;
    private float timer;

    void Awake()
    {
        if (!navAgent) navAgent = GetComponent<NavMeshAgent>();
        if (navAgent)
        {
            navAgent.updateRotation = false;
        }
    }

    void Start()
    {
        playerT = player ? player.transform : GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            index = Random.Range(0, patrolPoints.Length);
            TeleportTo(patrolPoints[index].position);
            FacePlayerInstant();
        }
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

            int next = 0;
            if (patrolPoints.Length > 1)
            {
                do { next = Random.Range(0, patrolPoints.Length); }
                while (next == index);
            }
            index = next;

            TeleportTo(patrolPoints[index].position);
            FacePlayerInstant(); 
        }
    }

    private void TeleportTo(Vector3 pos)
    {
        if (navAgent != null && navAgent.enabled) navAgent.Warp(pos);
        else transform.position = pos;
    }

    private void FacePlayerInstant()
    {
        Vector3 dir = playerT.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
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
