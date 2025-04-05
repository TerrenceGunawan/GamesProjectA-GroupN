using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public Transform hidePosition;
    public Vector3 exitOffset = new Vector3(0, 0, 1f);
    public GameObject interactionUI;
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
                interactionUI.SetActive(true);
            }
            else
            {
                player.HideAtPosition(hidePosition);
                interactionUI.SetActive(false);
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
}
