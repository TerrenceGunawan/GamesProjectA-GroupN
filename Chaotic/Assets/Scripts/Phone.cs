using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Phone : MonoBehaviour
{
    [SerializeField] private List<string> dialogue = new List<string>();
    [SerializeField] private List<float> delay = new List<float>();
    [SerializeField] private TextMeshProUGUI subtitles;
    [SerializeField] private GameObject interactText;
    private bool inReach = false;
    private bool pickedUp = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E) && !pickedUp)
        {
            StartCoroutine(ChangeSubtitles());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !pickedUp)
        {
            inReach = true;
            interactText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            interactText.SetActive(false);
        }
    }

    IEnumerator ChangeSubtitles()
    {
        pickedUp = true;
        for (int i = 0; i < dialogue.Count; i++)
        {
            subtitles.text = dialogue[i];
            yield return new WaitForSeconds(delay[i]);
        }
        subtitles.text = "";
    }
}
