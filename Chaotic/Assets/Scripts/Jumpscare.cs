using UnityEngine;
using UnityEngine.Video;

public class Jumpscare : MonoBehaviour
{
    private AudioSource audio;
    private VideoPlayer video;
    private bool played = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        video = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {

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
}