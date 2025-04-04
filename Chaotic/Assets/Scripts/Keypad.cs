using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    public Text keypadDisplay;  // Text to display the input
    public string correctCode = "12345";  // The correct code to enter
    private string currentInput = "";  // The player's current input

    public GameObject player;  // Reference to the player object
    public GameObject hud;     // HUD UI to toggle visibility
    public GameObject inv;     // Inventory UI to toggle visibility

    public AudioSource buttonSound;  // Button press sound
    public AudioSource correctSound;  // Correct code sound
    public AudioSource wrongSound;  // Wrong code sound

    // Disable the player's movement and control while interacting with the keypad
    public void DisablePlayerMovement()
    {
        // Assuming you're using Rigidbody-based movement, we disable the Rigidbody's movement
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // Stop physics-based movement
        }

        // Optionally, disable the player's ability to move or rotate via other methods
        player.GetComponent<Player>().enabled = false;  // Disable player controls (optional, based on your player script)
        
        // Hide the HUD and inventory while interacting
        hud.SetActive(false);
        inv.SetActive(false);

        // Show the cursor and unlock it
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Enable player movement when exiting keypad interaction
    public void EnablePlayerMovement()
    {
        // Re-enable the Rigidbody's movement if you're using it for player movement
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;  // Re-enable physics-based movement
        }

        // Optionally, re-enable player controls
        player.GetComponent<Player>().enabled = true;  // Re-enable player controls

        // Show the HUD and inventory again
        hud.SetActive(true);
        inv.SetActive(true);

        // Lock the cursor again and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Logic for adding numbers to the display
    public void AddNumber(int number)
    {
        currentInput += number.ToString();
        keypadDisplay.text = currentInput;  // Update the text display
        buttonSound.Play();  // Play button press sound
    }

    // Logic for clearing the input
    public void ClearInput()
    {
        currentInput = "";
        keypadDisplay.text = "";  // Clear the display
        buttonSound.Play();  // Play button press sound
    }

    // Logic for submitting the input
    public void SubmitInput()
    {
        if (currentInput == correctCode)
        {
            correctSound.Play();  // Play the correct sound
            keypadDisplay.text = "Correct Code!";
            Debug.Log("Access Granted");
            // Add code to trigger what happens when the correct code is entered (e.g., open door)
        }
        else
        {
            wrongSound.Play();  // Play the wrong sound
            keypadDisplay.text = "Wrong Code!";
            Debug.Log("Access Denied");
        }

        // Clear the input field
        currentInput = "";
    }

    // Close the keypad UI and re-enable player movement
    public void CloseKeypad()
    {
        keypadDisplay.text = "";  // Clear the display
        gameObject.SetActive(false);  // Hide the keypad UI
        EnablePlayerMovement();  // Re-enable player movement after interaction
    }
}
