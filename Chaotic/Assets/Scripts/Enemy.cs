using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Player player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private GameObject jumpscareEffect;
    [SerializeField] private AudioSource jumpscareAudio;
    [SerializeField] private AudioClip jumpscareSound;

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

        Teleport();
        teleportInterval = Random.Range(0.5f, maxTeleportInterval);
    }

    void Update()
    {
        if (player.EnemyVisible)
        {
            float chance;
            chance = Random.Range(0f, 1f);
            if (chance < 0.5f)
                FacePlayerSmooth();
        }
        
        timer += Time.deltaTime;
        if (timer >= teleportInterval)
        {
            Teleport();
        }
    }

    public void Teleport()
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

        transform.position = patrolPoints[index].position;
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

    public IEnumerator Jumpscare()
    {
        if (jumpscareEffect)
        {
            jumpscareEffect.SetActive(true);
            if (jumpscareAudio) jumpscareAudio.PlayOneShot(jumpscareSound);
            yield return new WaitForSeconds(1.5f);
            jumpscareEffect.SetActive(false);
        }
    }
}