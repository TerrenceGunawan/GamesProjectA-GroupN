using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Doors : MonoBehaviour, IInteractable
{
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Keypad keypad;
    private ItemChecker itemChecker;
    private GridChecker gridChecker;
    public Animator door;
    [SerializeField] private AudioSource lockedSound;
    [SerializeField] private AudioSource openSound;
    public bool DoorIsOpen = false;

    void Start()
    {
        itemChecker = GetComponent<ItemChecker>();
        gridChecker = GetComponent<GridChecker>();
    }

    public void Interact()
    {
        if ((keypad != null && keypad.Completed) || (itemChecker != null && itemChecker.HasSucceeded) || (gridChecker != null && gridChecker.AllItemsChecked))
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
        else if ((keypad != null && !keypad.Completed) || (itemChecker != null && !itemChecker.HasSucceeded) || (gridChecker != null && !gridChecker.AllItemsChecked))
        {
            if (keypad != null)
            {
                interactText.text = "I need to enter a code into the keypad."; // show "locked" text
            }
            else if (gridChecker != null)
            {
                interactText.text = "I need to solve a puzzle first."; // show "locked" text
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
        
    }

    public void DoorOpens()
    {
        door.SetBool("Open", true);
        DoorIsOpen = true;
        if (openSound != null) 
        {
            openSound.Play();
        }
    }

    void DoorCloses()
    {
        door.SetBool("Open", false);
        DoorIsOpen = false;
    }

    IEnumerator HideLockedTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = ""; // Clear the interaction text
    }
}