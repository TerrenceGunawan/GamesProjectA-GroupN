using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject saveIcon;
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private ItemChecker groundDoorKey;
    [SerializeField] private Phone phoneFinal;
    [SerializeField] private UIVideoPlayer finale;
    [SerializeField] private List<string> goals = new List<string>();
    public List<ItemChecker> checkers = new List<ItemChecker>();
    public List<ItemCheckerChecker> itemCheckerCheckers = new List<ItemCheckerChecker>();
    public List<Keypad> keypads = new List<Keypad>();
    public List<PatternChecker> patternCheckers = new List<PatternChecker>();
    public List<Jumpscare> jumpscares = new List<Jumpscare>();
    public List<Doors> doors = new List<Doors>();
    public List<Phone> phones = new List<Phone>();
    public List<GameObject> PlayerInventory = new List<GameObject>();
    private bool done = false;
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    // To track which items have already been counted
    private List<string> countedPuzzles = new List<string>();
    private List<string> countedObjects = new List<string>();
    private List<string> countedPhoneTwice = new List<string>();
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (groundDoorKey != null && groundDoorKey.HasSucceeded)
        {
            SceneManager.LoadScene("GroundFloor");
        }
        if (phoneFinal != null && phoneFinal.PickedUpTwice)
        {
            StartCoroutine(Wait(53f));
            Player player = FindFirstObjectByType<Player>();
            player.OnDisable();
        }
        // Count each checker only once
        foreach (ItemChecker checker in checkers)
        {
            if (checker.HasSucceeded && !countedPuzzles.Contains(checker.name))
            {
                countedPuzzles.Add(checker.name);
                StartCoroutine(SaveIcon());
            }
        }
        // Count each itemCheckerChecker only once
        foreach (ItemCheckerChecker icc in itemCheckerCheckers)
        {
            if (icc.AllItemsChecked && !countedPuzzles.Contains(icc.name))
            {
                countedPuzzles.Add(icc.name);
                StartCoroutine(SaveIcon());
            }
        }
        // Count each keypad only once
        foreach (Keypad kp in keypads)
        {
            if (kp.Completed && !countedPuzzles.Contains(kp.name))
            {
                countedPuzzles.Add(kp.name);
                StartCoroutine(SaveIcon());
            }
        }
        // Count each pattern checker only once
        foreach (PatternChecker pc in patternCheckers)
        {
            if (pc.Completed && !countedPuzzles.Contains(pc.name))
            {
                countedPuzzles.Add(pc.name);
                StartCoroutine(SaveIcon());
            }
        }
        // Count each object only once
        foreach (Jumpscare js in jumpscares)
        {
            if (js.Played && !countedObjects.Contains(js.name))
            {
                countedObjects.Add(js.name);
            }
        }
        foreach (Doors door in doors)
        {
            if (door.DoorIsOpen && !countedObjects.Contains(door.name))
            {
                countedObjects.Add(door.name);
            }
        }
        foreach (Phone phone in phones)
        {
            if (phone.PickedUp && !countedObjects.Contains(phone.name))
            {
                countedObjects.Add(phone.name);
            }
            if (phone.PickedUpTwice && !countedPhoneTwice.Contains(phone.name))
            {
                countedPhoneTwice.Add(phone.name);
            }
        }
        objectivesText.text = goals[countedPuzzles.Count];
    }

    public static void SaveGame()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        Player player = FindFirstObjectByType<Player>();

        PlayerSaveData data = new PlayerSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            sanity = player.Sanity,
            position = new float[] {
                player.transform.position.x,
                player.transform.position.y,
                player.transform.position.z
            },
            rotation = new float[] {
                player.transform.rotation.eulerAngles.x,
                player.transform.rotation.eulerAngles.y,
                player.transform.rotation.eulerAngles.z
            },
            inventory = new List<string>(player.Inventory),

            // Convert objects to names
            completedPuzzles = gm.countedPuzzles,
            completedObjects = gm.countedObjects,
            completedPhoneTwice = gm.countedPhoneTwice
        };

        ItemInteract[] allItems = FindObjectsByType<ItemInteract>(FindObjectsSortMode.None);
        foreach (ItemInteract item in allItems)
        {
            Debug.Log("Saving position for " + item.name);
            ItemTransformData itemData = new ItemTransformData
            {
                itemName = item.name,
                position = new float[] {
                    item.transform.position.x,
                    item.transform.position.y,
                    item.transform.position.z
                }
            };

            data.itemTransforms.Add(itemData);
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        Debug.Log("Saved to " + SavePath);
    }

    private IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (finale != null)
        {
            finale.gameObject.SetActive(true);
        }
    }

    private IEnumerator SaveIcon()
    {
        SaveGame();
        saveIcon.SetActive(true);
        yield return new WaitForSeconds(2f);
        saveIcon.SetActive(false);
    }
}
