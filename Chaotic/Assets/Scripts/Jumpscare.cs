using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class Jumpscare : MonoBehaviour
{
    [SerializeField] private ItemChecker item;
    [SerializeField] private PatternChecker pattern;
    [SerializeField] private Keypad keypad;
    private AudioSource audio;
    private VideoPlayer video;
    private Renderer renderer;
    private ParticleSystem particles;
    private bool played = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        video = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        particles = GetComponent<ParticleSystem>();
        if (audio == null && video == null)
        {
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
    }

    void OnBecameVisible()
    {
        if (audio != null || video != null)
        {
            return; // Do nothing if no audio or video is present
        }
        StartCoroutine(HideScare(2.5f)); // Hide the jumpscare after 1.5 seconds
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
        renderer.enabled = false;
        particles.Stop();
    }
    void Return()
    {
        renderer.enabled = true;
        particles.Play();
    }
}