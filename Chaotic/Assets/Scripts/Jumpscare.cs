using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class Jumpscare : MonoBehaviour
{
    [SerializeField] private ItemChecker item;
    [SerializeField] private GridChecker grid;
    [SerializeField] private Keypad keypad;
    private AudioSource audio;
    private VideoPlayer video;
    private Renderer renderer;
    private bool played = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        video = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        if (audio == null && video == null)
        {
            renderer.enabled = false; // Disable the renderer if no audio or video is present
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (item != null && item.HasSucceeded && !played)
        {
            played = true;
            renderer.enabled = true;
        }
        else if (grid != null && grid.AllItemsChecked && !played)
        {
            played = true;
            renderer.enabled = true;
        }
        else if (keypad != null && keypad.Completed && !played)
        {
            played = true;
            renderer.enabled = true;
        }
    }

    void OnBecameVisible()
    {
        if (audio != null || video != null)
        {
            return; // Do nothing if no audio or video is present
        }
        StartCoroutine(HideScare(1.5f)); // Hide the jumpscare after 1.5 seconds
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
        renderer.enabled = false;
    } 
}