using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ItemChecker : MonoBehaviour, IInteractable
{
    public List<string> ItemsNeeded = new List<string>();
    private List<string> remainingItems = new List<string>();
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI timedText;
    [SerializeField] private string successText;
    [SerializeField] private bool upgraded = false;
    public bool HasSucceeded = false;
    private Doors door;
    private GameObject otherObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        door = GetComponent<Doors>();
    }

    public void Interact()
    {
        if (upgraded)
        {
            return;
        }
        if (Check() && !HasSucceeded)
            {
                HasSucceeded = true; // Mark as done
                if (door == null)
                {
                    timedText.text = successText;
                    if (animator != null)
                    {
                        animator.SetBool("animate", true);
                    }
                    StartCoroutine(HideTextAfterSeconds(3f));
                }
                else
                {
                    interactText.text = "";
                    door.DoorOpens();
                }
                MusicManager.Instance.PlaySuccessMusic();
            }
            else if (!Check() && door == null)
            {
                timedText.text = "You don't have the right items.";
                StartCoroutine(HideTextAfterSeconds(3f));
            }
            else if (!Check() && door != null)
            {
                timedText.text = "I still need the " + FormatItemList(remainingItems) + "."; // show "locked" text and missing items;
                StartCoroutine(HideTextAfterSeconds(3f));
            }
    }

    public void OnRaycastHit()
    {
        if (upgraded)
        {
            return;
        }
        else if (!HasSucceeded && door == null)
        {
            interactText.text = "Interact";
        }
        else if (!HasSucceeded && door != null)
        {
            interactText.text = door.DoorIsOpen ? "Close" : "Open";
        }
    }

    // Update is called once per frame
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
        remainingItems = new List<string>();
        foreach (string item in ItemsNeeded)
        {
            if (!player.Inventory.Contains(item))
            {
                remainingItems.Add(item);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        otherObject = other.gameObject;
        if (upgraded)
        {
            if (other != null)
            {
                other.transform.position = new Vector3 (transform.position.x, other.transform.position.y, transform.position.z);
            }
            if (otherObject.name == ItemsNeeded.FirstOrDefault())
            {
                HasSucceeded = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        HasSucceeded = false;
        otherObject = null;
    }

    bool Check()
    {
        return ItemsNeeded.All(item => player.Inventory.Contains(item));
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

    private IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        timedText.text = "";
    }
}
