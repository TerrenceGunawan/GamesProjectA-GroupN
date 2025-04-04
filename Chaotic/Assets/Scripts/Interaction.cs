using UnityEngine;

public class Interaction : MonoBehaviour
{
    // Reference to the player (could be assigned through collision trigger)
    public GameObject player;  
    public string interactButton = "E";  // Button to interact (default "E")

    // Base method for interacting with any object
    public virtual void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

    // Detect the player entering the interaction range
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }

    // Detect the player exiting the interaction range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    // Update method to check for the interaction button press
    void Update()
    {
        if (player != null && Input.GetKeyDown(interactButton))
        {
            Interact();  // Call the Interact method when the player presses the interaction button
        }
    }
}
