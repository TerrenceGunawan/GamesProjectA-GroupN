using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class Jumpscare : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform enemyRaycastPoint;
    [SerializeField] private ItemChecker item;
    [SerializeField] private PatternChecker pattern;
    [SerializeField] private Keypad keypad;
    private AudioSource audio;
    private VideoPlayer video;
    private Collider collider;
    private Renderer renderer;
    private ParticleSystem particles;
    private bool played = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        video = GetComponent<VideoPlayer>();
        if (audio == null && video == null)
        {
            collider = GetComponent<Collider>();
            renderer = GetComponent<Renderer>();
            particles = GetComponent<ParticleSystem>();
            Remove();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (item != null && item.HasSucceeded && !played)
        {
            played = true;
            Return();
        }
        else if (pattern != null && pattern.Completed && !played)
        {
            played = true;
            Return();
        }
        else if (keypad != null && keypad.Completed && !played)
        {
            played = true;
            Return();
        }
        if (video == null && audio == null && played)
        {
            Vector3 dirToEnemy = (enemyRaycastPoint.position - camera.transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(camera.transform.position, enemyRaycastPoint.position);
            // Raycast from camera to enemy
            if (Physics.Raycast(camera.transform.position, dirToEnemy, out RaycastHit hit, distanceToEnemy))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    if (renderer != null && renderer.isVisible)
                    {
                        StartCoroutine(HideScare(0.5f)); // Hide the jumpscare after 2.5 seconds
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() && !played)
        {
            if (audio != null)
            {
                audio.Play();
                played = true;
            }
            else if (video != null)
            {
                video.Play();
                played = true;
            }
        }
    }

    private IEnumerator HideScare(float delay)
    {
        yield return new WaitForSeconds(delay);
        Remove();
    }

    void Remove()
    {
        collider.enabled = false;
        renderer.enabled = false;
        particles.Stop();
    }
    void Return()
    {
        collider.enabled = true;
        renderer.enabled = true;
        particles.Play();
    }
}