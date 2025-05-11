using UnityEngine;
using TMPro; // Add this if you're using TextMeshPro

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private Vector3 exitOffset = new Vector3(0, 0, 1f);
    [SerializeField] private TextMeshProUGUI interactionText; // Actual text component

    private Transform hidePosition;
    private Player player;
    private bool playerInRange = false;

    private Enemy enemy;

    void Start()
    {
        hidePosition = transform;
        enemy = FindObjectOfType<Enemy>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
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
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            if (player != null)
            {
                playerInRange = true;
                UpdateInteractionText(player.IsHidden());
                interactionText.enabled = true;
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
