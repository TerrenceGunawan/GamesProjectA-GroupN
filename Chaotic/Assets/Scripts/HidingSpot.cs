using UnityEngine;
using TMPro; // Add this if you're using TextMeshPro

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private Vector3 exitOffset = new Vector3(0, 0, 1f);
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
            if (player.IsHidden())
            {
                player.ExitHiding(exitOffset);
                UpdateInteractionText(false);
            }
            else
            {
                player.HideAtPosition(hidePosition);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            if (player != null)
            {
                inReach = true;
                UpdateInteractionText(player.IsHidden());
                interactionText.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            if (player != null && !player.IsHidden())
            {
                inReach = false;
                interactionText.enabled = false;
                player = null;
            }
        }
    }

    void UpdateInteractionText(bool isHiding)
    {
        if (interactionText != null)
        {
            interactionText.text = isHiding ? "Exit [E]" : "Hide [E]";
        }
    }
}
