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
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private string successText;
    public bool HasSucceeded = false;
    private Doors door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        door = GetComponent<Doors>();
    }

    public void Interact()
    {
        if (Check() && !HasSucceeded)
        {
            HasSucceeded = true; // Mark as done
            if (door == null)
            {
                interactText.text = successText;
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
            interactText.text = "You don't have the right items.";
            StartCoroutine(HideTextAfterSeconds(3f));
        }
        else if (!Check() && door != null)
        {
            interactText.text = "I still need the " + FormatItemList(remainingItems) + "."; // show "locked" text and missing items;
            StartCoroutine(HideTextAfterSeconds(3f));
        }
    }

    public void OnRaycastHit()
    {
        if (!HasSucceeded && door == null)
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
        remainingItems = new List<string>();
        foreach (string item in ItemsNeeded)
        {
            if (!player.Inventory.Contains(item))
            {
                remainingItems.Add(item);
            }
        }
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

    IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = "";
    }
}
