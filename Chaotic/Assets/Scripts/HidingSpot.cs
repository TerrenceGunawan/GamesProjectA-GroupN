using UnityEngine;
using TMPro; // Add this if you're using TextMeshPro

public class HidingSpot : MonoBehaviour
{
    public Transform hidePosition;
    public Vector3 exitOffset = new Vector3(0, 0, 1f);
    public GameObject interactionUI; // Canvas object
    public TextMeshProUGUI interactionText; // Actual text component
    public KeyCode interactKey = KeyCode.E;

    private Player player;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (player.IsHidden())
            {
                player.ExitHiding(exitOffset);
                UpdateInteractionText(false);
                interactionUI.SetActive(true);
            }
            else
            {
                player.HideAtPosition(hidePosition);
                interactionUI.SetActive(false); // Hide while hidden
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            if (player != null)
            {
                playerInRange = true;
                UpdateInteractionText(player.IsHidden());
                interactionUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player != null && !player.IsHidden())
            {
                playerInRange = false;
                interactionUI.SetActive(false);
                player = null;
            }
        }
    }

    void UpdateInteractionText(bool isHiding)
    {
        if (interactionText != null)
        {
            interactionText.text = isHiding ? "Press E to exit" : "Press E to hide";
        }
    }
}
