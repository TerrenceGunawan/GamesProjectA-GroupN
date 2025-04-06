using UnityEngine;

public class ItemInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject interactText;
    [SerializeField] private GameObject description;
    private bool inReach = false;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            interactText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            interactText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E) && !description.activeSelf)
        {
            description.SetActive(true);
            interactText.SetActive(false);
            player.DisableMovement();
        }
        else if (inReach && Input.GetKeyDown(KeyCode.E) && description.activeSelf)
        {
            description.SetActive(false);
            interactText.SetActive(true);
            player.EnableMovement();
        }
    }
}
