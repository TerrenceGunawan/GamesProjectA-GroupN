using UnityEngine;

public class OpenKeyPad : MonoBehaviour
{
    public GameObject keypadOB;  // Reference to the keypad canvas
    public GameObject keypadText;  // The text UI that prompts interaction

    public bool inReach;  // Tracks if the player is in reach of the keypad

    public Player player;  // Reference to the player script for movement control

    void Start()
    {
        inReach = false;  // Initially, the player is not in reach of the keypad
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  // When the player enters the trigger
        {
            inReach = true;
            keypadText.SetActive(true);  // Show interaction text
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  // When the player exits the trigger
        {
            inReach = false;
            keypadText.SetActive(false);  // Hide interaction text
        }
    }

    void Update()
    {
        // If the player presses the interact button and is within reach
        if (Input.GetButtonDown("Interact") && inReach)
        {
            OpenKeypadUI();  // Call method to open the keypad UI
        }
    }

    // Open the keypad UI and disable player movement
    void OpenKeypadUI()
    {
        keypadOB.SetActive(true);  // Show the keypad UI
        player.OnDisable();  // Disable player movement when interacting
    }
}
