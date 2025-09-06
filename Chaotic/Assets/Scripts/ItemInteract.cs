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
    [SerializeField] private TextMeshProUGUI timedText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RawImage image;
    [SerializeField] private Texture itemTexture;
    [SerializeField] private string itemDesc;
    [SerializeField] private bool takeAble;
    [SerializeField] private float magneticForce = 10f;
    [SerializeField] private bool sanityRegain;
    [SerializeField] private AudioClip sanityClip;
    private AudioSource source;
    public bool Taken = false;
    public bool Movable = false;
    public bool Magnet = false;
    private bool regainCheck = false;
    private Rigidbody rb;
    private Vector3 originalPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
    }

    void Start()
    {
        if (sanityRegain)
        source = player.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && description != null && description.activeInHierarchy)
        {
            Exit();
        }
        if (timedText != null && timedText.text != "")
        {
            Debug.Log("Hiding interact text");
            interactText.enabled = false;
        }
        else
        {
            interactText.enabled = true;
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
            timedText.text = "You got " + gameObject.name;
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
        if (Movable && rb.linearVelocity == Vector3.zero)
        {
            interactText.text = "Grab";
        }
        else if (!Movable)
        { 
            interactText.text = "Interact";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Reset"))
        {
            transform.position = originalPosition; // Reset position if it collides with a reset object
        }
    }

    void OnTriggerStay(Collider other)
    {
        ItemInteract otherItem = other.GetComponent<ItemInteract>();
        if (otherItem != null && Magnet && otherItem.Magnet)
        {
            Transform otherItemTF = other.GetComponent<Transform>();
            float distance = (transform.position - otherItemTF.position).magnitude;
            Vector3 direction = otherItemTF.position - transform.position;
            other.GetComponent<Rigidbody>().AddForce(-direction * magneticForce / distance);
            if (distance < 0.2f)
            {
                rb.AddForce(direction * magneticForce / distance);
            }
        }
    }

    private IEnumerator HideTextAfterSeconds(float delay)
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(delay);
        timedText.text = "";
    }

    private IEnumerator PlayVoiceLine()
    {
        player.RegainSanity();
        regainCheck = true;
        source.loop = false;
        source.Stop();
        source.volume = 0.4f;
        source.PlayOneShot(sanityClip);
        yield return new WaitForSeconds(2f);
        source.volume = 1;
        source.loop = true;
    }
}
