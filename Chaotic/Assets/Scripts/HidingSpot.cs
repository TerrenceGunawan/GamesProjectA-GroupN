using UnityEngine;
using TMPro; // Add this if you're using TextMeshPro

public class HidingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private TextMeshProUGUI interactText; // Actual text component
    [SerializeField] private Player player;

    private Transform hidePosition;
    private AudioSource audio;

    void Start()
    {
        hidePosition = transform;
        audio = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (player.IsHidden)
            {
                player.ExitHiding();
                audio.Pause();
                interactText.text = ""; // Clear the interaction text
            }
            else
            {
                player.HideAtPosition(hidePosition);
                audio.Play();
                UpdateInteractionText();
            }
    }

    public void OnRaycastHit()
    {
        UpdateInteractionText();
    }

    void Update()
    {
    
    }

    void UpdateInteractionText()
    {
        interactText.text = player.IsHidden ? "Exit" : "Hide";
    }
}
