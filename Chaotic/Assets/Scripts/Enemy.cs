using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Player player;    
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform transform; 

    [Header("Behaviour")]
    [SerializeField] private float teleportInterval = 2.5f;
    [SerializeField] private float turnSpeed = 360f;       

    private Transform playerT;
    private int index = -1;
    private float timer;

    void Awake()
    {
        if (!transform) transform = GetComponent<Transform>();
    
    }

    void Start()
    {
        playerT = player ? player.transform : GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            index = Random.Range(0, patrolPoints.Length);
            TeleportTo(patrolPoints[index].position);
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
