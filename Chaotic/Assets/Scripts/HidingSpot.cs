using UnityEngine;
using TMPro; // Add this if you're using TextMeshPro

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionText; // Actual text component
    [SerializeField] private Player player;

    private Transform hidePosition;
    private bool inReach = false;

    void Start()
    {
        hidePosition = transform;
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            if (player.IsHidden)
            {
                player.ExitHiding();
                inReach = false;
                interactionText.text = ""; // Clear the interaction text
            }
            else
            {
                player.HideAtPosition(hidePosition);
                UpdateInteractionText();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            UpdateInteractionText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            if (player != null && !player.IsHidden)
            {
                inReach = false;
                interactionText.text = ""; // Clear the interaction text
            }
        }
    }

    void UpdateInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.text = player.IsHidden ? "Exit [E]" : "Hide [E]";
        }
    }
}
