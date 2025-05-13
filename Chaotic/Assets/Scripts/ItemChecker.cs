using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    [SerializeField] private List<string> itemsNeeded = new List<string>();
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private string successText;
    [SerializeField] private GameObject reward;
    public bool HasSucceeded = false;
    private bool inReach = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reward.SetActive(false); // Hide the reward at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E) && Check(itemsNeeded, player.Inventory) && !HasSucceeded)
        {
            HasSucceeded = true; // Mark as done
            interactText.text = "";
            reward.SetActive(true); // Show the reward
            GetComponent<GameObject>().SetActive(false); // Hide the item checker
            if (GetComponent<Doors>() == null)
            {
                interactText.text = successText;
                StartCoroutine(HideTextAfterSeconds(3f));            
            }
            MusicManager.Instance.PlaySuccessMusic();
        }
        else if (inReach && Input.GetKeyDown(KeyCode.E) && !Check(itemsNeeded, player.Inventory) && GetComponent<Doors>() == null)
        {
            interactText.text = "You don't have the right items.";
            StartCoroutine(HideTextAfterSeconds(3f));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !HasSucceeded)
        {
            inReach = true;
            if (GetComponent<Doors>() == null)
            {
                interactText.text = "Interact [E]";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !HasSucceeded)
        {
            inReach = false;
            interactText.text = ""; // Clear the interaction text
        }
    }

    bool Check(List<string> list1, List<string> list2)
    {
        return list1.All(item => list2.Contains(item));
    }

    IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = "";
    }
}
