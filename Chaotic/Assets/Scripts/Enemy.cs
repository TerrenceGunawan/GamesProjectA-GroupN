using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private Player player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private GameObject jumpscareEffect;
    [SerializeField] private AudioSource jumpscareAudio;
    [SerializeField] private AudioClip jumpscareSound;

    [Header("Behaviour")]
    [SerializeField] private float maxTeleportInterval = 2.5f;
    [SerializeField] private float turnSpeed = 360f;

    private Transform transform;
    private Animator animator;
    private Transform playerT;
    private float teleportInterval;
    public float Chance;
    private int index = -1;
    private float timer = 0f;

    void Awake()
    {
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        playerT = player ? player.transform : GameObject.FindGameObjectWithTag("Player")?.transform;

        Teleport();
        teleportInterval = Random.Range(0.5f, maxTeleportInterval);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= teleportInterval)
        {
            Teleport();
            Chance = Random.Range(0f, 1f);
        }
        if (Chance < 0.5f)
            FacePlayerSmooth();
    }

    public void Teleport()
    {
        animator.ResetTrigger("run");
        animator.SetTrigger("stand");
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

    public IEnumerator RunAtPlayer()
    {
        animator.ResetTrigger("stand");
        animator.SetTrigger("run");

        float duration = 1f;
        float timer = 0f;
        float runSpeed = 10f;
        enemyCollider.enabled = false;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Recalculate direction to player
            Vector3 dir = playerT.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                var target = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);

                // Move smoothly toward player
                transform.position += dir.normalized * runSpeed * Time.deltaTime;
            }

            yield return null; // wait one frame
        }
        jumpscareAudio.PlayOneShot(jumpscareSound);
        enemyCollider.enabled = true;
        Teleport();
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