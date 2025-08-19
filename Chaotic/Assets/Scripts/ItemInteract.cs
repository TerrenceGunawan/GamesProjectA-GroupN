using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ItemInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject sanityBar;
    [SerializeField] private GameObject objective;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RawImage image;
    [SerializeField] private Texture itemTexture;
    [SerializeField] private string itemDesc;
    [SerializeField] private bool takeAble;
    [SerializeField] private bool sanityRegain;
    [SerializeField] private AudioClip sanityClip;
    private AudioSource source;
    private AudioClip playerClip;
    public bool Taken = false;
    public bool Movable = false;
    private bool regainCheck = false;

    void Start()
    {
        if (source != null)
        {
            source = player.GetComponent<AudioSource>();
            playerClip = source.clip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && description != null && description.activeInHierarchy)
        {
            Exit();
        }
    }

    void Exit()
    {
        player.SetPauseFunction();
        crosshair.SetActive(true);  // Show the crosshair again
        objective.SetActive(true);
        sanityBar.SetActive(true);
        description.SetActive(false);
        sanityBar.SetActive(true);
        interactText.text = "Interact";
        player.EnableMovement();
    }

    public void Interact()
    {
        // Item is takeable
            if (takeAble)
            {
                Taken = true;
                interactText.text = "You got " + gameObject.name;
                player.AddInventory(gameObject.name);
                StartCoroutine(HideTextAfterSeconds(2f));
            }
            // Not takeable, but has a description
            else if (description != null && !description.activeSelf)
            {
                player.SetPause = true;
                crosshair.SetActive(false);  // Hide the crosshair when interacting
                objective.SetActive(false);
                sanityBar.SetActive(false);
                description.SetActive(true);
                sanityBar.SetActive(false);
                if (!regainCheck && sanityRegain)
                {
                    StartCoroutine(PlayVoiceLine());
                }
                descriptionText.text = itemDesc;
                interactText.text = "";
                if (image != null && itemTexture != null)
                {
                    image.texture = itemTexture;

                    // Get parent size (the container the image is in)
                    RectTransform parent = image.transform.parent.GetComponent<RectTransform>();
                    RectTransform rt = image.GetComponent<RectTransform>();

                    float parentWidth = parent.rect.width;
                    float parentHeight = parent.rect.height;

                    float textureRatio = (float)itemTexture.width / itemTexture.height;
                    float parentRatio = parentWidth / parentHeight;

                    Vector2 newSize;

                    if (textureRatio > parentRatio)
                    {
                        // Fit to width
                        newSize = new Vector2(parentWidth, parentWidth / textureRatio);
                    }
                    else
                    {
                        // Fit to height
                        newSize = new Vector2(parentHeight * textureRatio, parentHeight);
                    }

                    rt.sizeDelta = newSize;
                }
                player.DisableMovement();
            }
            // Description is currently active, so hide it
            else if (description != null && description.activeSelf)
            {
                Exit();
            }
    }

    public void OnRaycastHit()
    {
        if (Movable)
        {
            interactText.text = "Grab";
        }
        else
        { 
            interactText.text = "Interact";
        }
    }

    IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = "";
        gameObject.SetActive(false);
    }

    IEnumerator PlayVoiceLine()
    {
        player.RegainSanity();
        regainCheck = true;
        source.clip = sanityClip;
        source.volume = 0.4f;
        source.Play();
        yield return new WaitForSeconds(2f);
        source.clip = playerClip;
        source.volume = 1;
        source.Stop();
    }
}
