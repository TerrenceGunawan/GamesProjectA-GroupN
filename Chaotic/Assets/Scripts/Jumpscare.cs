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
    public bool Played = false;

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
        if (item != null && item.HasSucceeded && !Played)
        {
            Played = true;
            Return();
        }
        else if (pattern != null && pattern.Completed && !Played)
        {
            Played = true;
            Return();
        }
        else if (keypad != null && keypad.Completed && !Played)
        {
            Played = true;
            Return();
        }
        if (video == null && audio == null && Played)
        {
            Vector3 dirToEnemy = (enemyRaycastPoint.position - camera.transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(camera.transform.position, enemyRaycastPoint.position);
            // Raycast from camera to enemy
            if (Physics.Raycast(camera.transform.position, dirToEnemy, out RaycastHit hit, distanceToEnemy))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    if (renderer != null)
                    {
                        // Convert enemy position to viewport space relative to player camera
                        Vector3 viewportPoint = camera.WorldToViewportPoint(renderer.bounds.center);

                        bool inView = viewportPoint.z > 0 &&       // In front of camera
                                    viewportPoint.x > 0 && viewportPoint.x < 1 &&
                                    viewportPoint.y > 0 && viewportPoint.y < 1;

                        if (inView)
                        {
                            StartCoroutine(HideScare(0.5f));
                        }
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() && !Played)
        {
            if (audio != null)
            {
                audio.Play();
                Played = true;
            }
            else if (video != null)
            {
                video.Play();
                Played = true;
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