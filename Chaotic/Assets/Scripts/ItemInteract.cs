using UnityEngine;
using System.Collections;
using TMPro;

public class ItemInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject description;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private string itemDesc;
    [SerializeField] private bool takeAble;
    [SerializeField] private bool sanityRegain;
    private bool inReach = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
    {
        // Item is takeable
        if (takeAble)
        {
            interactText.text = "";
            if (sanityRegain)
            {
                player.Sanity += 15f;
                interactText.text = itemDesc;
            }
            else
            {
                interactText.text = "You got " + itemDesc;
                player.AddInventory(itemDesc);
            }
            StartCoroutine(HideTextAfterSeconds(2f));
        }
        // Not takeable, but has a description
        else if (description != null && !description.activeSelf)
        {
            crosshair.SetActive(false);  // Hide the crosshair when interacting
            description.SetActive(true);
            descriptionText.text = itemDesc;
            interactText.text = "";
            player.DisableMovement();
        }
        // Description is currently active, so hide it
        else if (description != null && description.activeSelf)
        {
            crosshair.SetActive(true);  // Show the crosshair again
            description.SetActive(false);
            interactText.text = "Interact [E]";
            player.EnableMovement();
        }
    }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
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
            interactText.text = ""; // Clear the interaction text
        }
    }

    IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = "";
        gameObject.SetActive(false);
    }
}
