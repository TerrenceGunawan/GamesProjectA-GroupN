using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Phone : MonoBehaviour, IInteractable
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
    [SerializeField] private ItemChecker itemChecker;
    [SerializeField] private ItemCheckerChecker itemCheckerChecker;

    private AudioSource audioSource;
    private bool pickedUp = false;
    private bool pickedUpTwice = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = phoneRing;
    }

    public void Interact()
    {
        if (!pickedUp && ((keypad != null && !keypad.Completed) || itemChecker != null && !itemChecker.HasSucceeded) || (itemCheckerChecker != null && !itemCheckerChecker.AllItemsChecked))
        {
            pickedUp = true;
            audioSource.clip = talking[0];
            audioSource.loop = false;
            audioSource.Play();
            interactText.text = "";
            StartCoroutine(ChangeSubtitles());
        }
        else if (!pickedUp && !pickedUpTwice && ((keypad != null && keypad.Completed) || itemChecker != null && itemChecker.HasSucceeded) || (itemCheckerChecker != null && itemCheckerChecker.AllItemsChecked))
        {
            pickedUpTwice = true;
            pickedUp = true;
            audioSource.clip = talking[1];
            audioSource.loop = false;
            audioSource.Play();
            interactText.text = "";
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ChangeSubtitles());
        }
        if ((keypad != null && keypad.Completed) || (itemChecker != null && itemChecker.HasSucceeded) || (itemCheckerChecker != null && itemCheckerChecker.AllItemsChecked))
        {
            pickedUp = false;
        }
    }

    public void OnRaycastHit()
    {
        interactText.text = "Interact";
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= ringDistance && !pickedUp && !pickedUpTwice)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = phoneRing;
                audioSource.Play();
                
            }
        }
    }

    private IEnumerator ChangeSubtitles()
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
