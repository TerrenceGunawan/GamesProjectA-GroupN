using UnityEngine;
using System.Collections;
using TMPro;

public class ItemInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject interactText;
    [SerializeField] private GameObject description;
    [SerializeField] private string itemDesc;
    [SerializeField] private TextMeshProUGUI item;
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
            interactText.SetActive(false);
            item.enabled = true;
            if (sanityRegain)
            {
                player.Sanity += 15f;
                item.text = itemDesc;
            }
            else
            {
                item.text = "You got " + itemDesc;
                player.AddInventory(itemDesc);
            }
            StartCoroutine(HideTextAfterSeconds(item, 2f));
        }
        // Not takeable, but has a description
        else if (description != null && !description.activeSelf)
        {
            description.SetActive(true);
            item.text = itemDesc;
            interactText.SetActive(false);
            player.DisableMovement();
        }
        // Description is currently active, so hide it
        else if (description != null && description.activeSelf)
        {
            description.SetActive(false);
            interactText.SetActive(true);
            player.EnableMovement();
        }
    }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
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

    IEnumerator HideTextAfterSeconds(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.enabled =false;
        gameObject.SetActive(false);
    }
}
