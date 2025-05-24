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
        else if (inReach && Input.GetKeyDown(KeyCode.E) && ((keypad != null && !keypad.Completed) || (itemChecker != null && !itemChecker.HasSucceeded)))
        {
            if (keypad != null)
            {
                interactText.text = "I need to enter a code into the keypad."; // show "locked" text
            }
            else if (itemChecker != null)
            {
                interactText.text = "I still need the " + FormatItemList(itemChecker.RemainingItems); // show "locked" text and missing items
            }
            StartCoroutine(HideLockedTextAfterSeconds(3f)); // hide after a short delay
            if (lockedSound != null)
            {
                lockedSound.Play();
            }
        }
    }

    public void DoorOpens()
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

    private string FormatItemList(List<string> items)
    {
        if (items.Count == 1)
        {
            return items[0];
        }
        else
        {
            string allButLast = string.Join(", ", items.GetRange(0, items.Count - 1));
            string last = items[items.Count - 1];
            return allButLast + ", and " + last;
        }
    }

    IEnumerator HideLockedTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = ""; // Clear the interaction text
    }
}