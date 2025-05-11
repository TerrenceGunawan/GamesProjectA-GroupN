using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Doors : MonoBehaviour
{
    [SerializeField] private GameObject lockedText;
    [SerializeField] private Keypad keypad;
    private ItemChecker itemChecker;
    public Animator door;
    public GameObject openText;
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
            openText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E) && ((keypad != null && keypad.Right) || (itemChecker != null && itemChecker.HasSucceeded)))
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
        else if (inReach && Input.GetKeyDown(KeyCode.E)  && ((keypad != null && !keypad.Right) || (itemChecker != null && !itemChecker.HasSucceeded)))
        {
            lockedText.SetActive(true); // show "locked" text
            StartCoroutine(HideLockedTextAfterSeconds(2f)); // hide after a short delay
            if (lockedSound != null) lockedSound.Play();
        }
    }

    void DoorOpens()
    {
        door.SetBool("Open", true);
        doorIsOpen = true;
        lockedText.SetActive(false);

        if (openSound != null) openSound.Play();
    }

    void DoorCloses()
    {
        door.SetBool("Open", false);
        doorIsOpen = false;
    }


    IEnumerator HideLockedTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        lockedText.SetActive(false);
    }

}