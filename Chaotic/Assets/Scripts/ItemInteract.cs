using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ItemInteract : MonoBehaviour
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
    public bool Taken = false;
    private bool inReach = false;
    private bool regainCheck = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && description != null && description.activeInHierarchy)
        {
            Exit();
        }

        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            // Item is takeable
            if (takeAble)
            {
                Taken = true;
                inReach = false;
                interactText.text = "You got " + gameObject.name;
                player.AddInventory(gameObject.name);
                StartCoroutine(HideTextAfterSeconds(2f));
            }
            // Not takeable, but has a description
            else if (description != null && !description.activeSelf)
            {
                player.setPause = true;
                crosshair.SetActive(false);  // Hide the crosshair when interacting
                objective.SetActive(false);
                sanityBar.SetActive(false);
                description.SetActive(true);
                sanityBar.SetActive(false);
                if (!regainCheck && sanityRegain)
                {
                    regainCheck = true;
                    player.RegainSanity();
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
    }

        void Exit()
    {
        player.setPauseFunction();
        crosshair.SetActive(true);  // Show the crosshair again
        objective.SetActive(true);
        sanityBar.SetActive(true);
        description.SetActive(false);
        sanityBar.SetActive(true);
        interactText.text = "Interact [E]";
        player.EnableMovement();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !Taken)
        {
            inReach = true;
            interactText.text = "Interact [E]";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !Taken)
        {
            inReach = false;
            interactText.text = ""; // Clear the interaction text
        }
    }

    IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactText.text = "";
        gameObject.SetActive(false);
    }
}
