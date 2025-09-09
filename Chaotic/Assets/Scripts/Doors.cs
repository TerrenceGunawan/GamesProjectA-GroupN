using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Doors : MonoBehaviour, IInteractable
{
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI timedText;
    [SerializeField] private Keypad keypad;
    private ItemChecker itemChecker;
    private PatternChecker patternChecker;
    public Animator door;
    [SerializeField] private AudioSource lockedSound;
    [SerializeField] private AudioSource openSound;
    public bool DoorIsOpen = false;

    void Start()
    {
        itemChecker = GetComponent<ItemChecker>();
        patternChecker = GetComponent<PatternChecker>();
    }

    public void Interact()
    {
        if ((keypad != null && keypad.Completed) || (itemChecker != null && itemChecker.HasSucceeded) || (patternChecker != null && patternChecker.Completed))
        {
            if (!DoorIsOpen)
            {
                DoorOpens();
            }
            else
            {
                DoorCloses();
            }
        }
        else if ((keypad != null && !keypad.Completed) || (itemChecker != null && !itemChecker.HasSucceeded) || (patternChecker != null && !patternChecker.Completed) || (keypad == null && itemChecker == null && patternChecker == null))
        {
            if (keypad != null)
            {
                timedText.text = "I need to enter a code into the keypad."; // show "locked" text
            }
            else if (itemChecker != null)
            {
                timedText.text = "I need a specific item to open this door."; // show "locked" text
            }
            else if (patternChecker != null)
            {
                timedText.text = "I need to solve a puzzle first."; // show "locked" text
            }
            else
            {
                timedText.text = "I can't go there."; // show "locked" text
            }
            StartCoroutine(HideLockedTextAfterSeconds(3f)); // hide after a short delay
            if (lockedSound != null)
            {
                lockedSound.Play();
            }
        }
    }

    public void OnRaycastHit()
    {
        interactText.text = DoorIsOpen ? "Close" : "Open";
    }

    void Update()
    {
        if (timedText != null && timedText.text != "")
        {
            interactText.text = "";
            interactText.gameObject.SetActive(false);
        }
        else if (timedText != null && timedText.text == "")
        {
            interactText.gameObject.SetActive(true);
        }
    }

    public void DoorOpens()
    {
        door.ResetTrigger("Close");
        door.SetTrigger("Open");
        DoorIsOpen = true;
        if (openSound != null) 
        {
            openSound.Play();
        }
    }

    void DoorCloses()
    {
        door.ResetTrigger("Open");
        door.SetTrigger("Close");
        DoorIsOpen = false;
    }

    private IEnumerator HideLockedTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        timedText.text = ""; // Clear the interaction text
    }
}