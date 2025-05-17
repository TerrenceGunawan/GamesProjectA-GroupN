using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Phone : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float ringDistance = 6f;
    [SerializeField] private List<string> dialogue = new List<string>();
    [SerializeField] private List<AudioClip> talking = new List<AudioClip>();
    [SerializeField] private List<float> delay = new List<float>();
    [SerializeField] private int dialogueLineStop;
    [SerializeField] private TextMeshProUGUI subtitles;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private AudioClip phoneRing;
    [SerializeField] private Keypad keypad;

    private AudioSource audioSource;
    private bool inReach = false;
    private bool pickedUp = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = phoneRing;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= ringDistance && !pickedUp)
        {
            if (!audioSource.isPlaying && HasLineOfSightToPlayer())
            {
                audioSource.clip = phoneRing;
                audioSource.Play();
                
            }
        }
        if (inReach && Input.GetKeyDown(KeyCode.E) && !pickedUp && (keypad != null && !keypad.Completed))
        {
            pickedUp = true;
            audioSource.clip = talking[0];
            audioSource.loop = false;
            audioSource.Play();
            interactText.text = "";
            StartCoroutine(ChangeSubtitles());
        }
        else if (inReach && Input.GetKeyDown(KeyCode.E)  && !pickedUp && (keypad != null && keypad.Completed))
        {
            pickedUp = true;
            audioSource.clip = talking[1];
            audioSource.loop = false;
            audioSource.Play();
            interactText.text = "";
            StartCoroutine(ChangeSubtitles());
        }
        if (keypad.Completed)
        {
            pickedUp = false;

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !pickedUp)
        {
            inReach = true;
            interactText.text = "Interact [E]";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            interactText.text = "";
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Ray ray = new Ray(transform.position, directionToPlayer.normalized);
        RaycastHit hit;

        // Check if ray hits anything between phone and player
        if (Physics.Raycast(ray, out hit, ringDistance))
        {
            // Only true if the player is what was hit
            return hit.transform == player;
        }

        return false;
    }


    IEnumerator ChangeSubtitles()
    {
        if (audioSource.clip == talking[0])
        {
            for (int i = 0; i < dialogueLineStop; i++)
            {
                subtitles.text = dialogue[i];
                yield return new WaitForSeconds(delay[i]);
            }
            subtitles.text = "";
        }
        else
        {
            for (int i = dialogueLineStop; i < dialogue.Count; i++)
            {
                subtitles.text = dialogue[i];
                yield return new WaitForSeconds(delay[i]);
            }
            subtitles.text = "";
        }
        
    }
}
