using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Phone : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float ringDistance = 5f;
    [SerializeField] private List<string> dialogue = new List<string>();
    [SerializeField] private List<float> delay = new List<float>();
    [SerializeField] private TextMeshProUGUI subtitles;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private AudioSource phoneRing;
    [SerializeField] private AudioSource talking;

    private bool inReach = false;
    private bool pickedUp = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= ringDistance && !pickedUp)
        {
            if (!phoneRing.isPlaying)
            {
                phoneRing.Play();
                phoneRing.loop = true;
            }
        }
        if (inReach && Input.GetKeyDown(KeyCode.E) && !pickedUp)
        {
            pickedUp = true;
            phoneRing.Stop();
            talking.Play();
            interactText.text = "";
            StartCoroutine(ChangeSubtitles());
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

    IEnumerator ChangeSubtitles()
    {
        for (int i = 0; i < dialogue.Count; i++)
        {
            subtitles.text = dialogue[i];
            yield return new WaitForSeconds(delay[i]);
        }
        subtitles.text = "";
    }
}
