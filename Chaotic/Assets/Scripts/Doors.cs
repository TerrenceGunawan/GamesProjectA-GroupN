using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Doors : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Keypad keypad;
    private ItemChecker itemChecker;
    public Animator door;
    [SerializeField] private AudioSource lockedSound;
    [SerializeField] private AudioSource openSound;
    public bool inReach;
    private bool doorIsOpen = false;

    void Start()
    {
        inReach = false;
        itemChecker = GetComponent<ItemChecker>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            interactText.text = doorIsOpen ? "Close [E]" : "Open [E]";

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            interactText.text = ""; // Clear the interaction text
        }
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E) && ((keypad != null && keypad.Completed) || (itemChecker != null && itemChecker.HasSucceeded)))
        {
            if (!doorIsOpen)
            {
                DoorOpens();
            }
            else
            {
                DoorCloses();
            }
        }
        else if (inReach && Input.GetKeyDown(KeyCode.E)  && ((keypad != null && !keypad.Completed) || (itemChecker != null && !itemChecker.HasSucceeded)))
        {            
            interactText.text = "The door is locked."; // show "locked" text
            StartCoroutine(HideLockedTextAfterSeconds(2f)); // hide after a short delay
            if (lockedSound != null) 
            {
                lockedSound.Play();
            }
        }
    }

    void DoorOpens()
    {
        door.SetBool("Open", true);
        doorIsOpen = true;
        if (openSound != null) 
        {
            openSound.Play();
        }
    }

    void DoorCloses()
    {
        door.SetBool("Open", false);
        doorIsOpen = false;
    }


    IEnumerator HideLockedTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = ""; // Clear the interaction text
    }
}