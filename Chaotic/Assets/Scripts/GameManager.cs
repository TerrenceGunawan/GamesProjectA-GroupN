using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private ItemChecker groundDoorKey;
    [SerializeField] private ItemChecker finalDoorKey;
    [SerializeField] private List<string> goals = new List<string>();
    [SerializeField] private List<ItemChecker> checkers = new List<ItemChecker>();
    [SerializeField] private List<ItemCheckerChecker> itemCheckerCheckers = new List<ItemCheckerChecker>();
    [SerializeField] private List<Keypad> keypads = new List<Keypad>();
    [SerializeField] private List<PatternChecker> patternCheckers = new List<PatternChecker>();
    private bool done = false;
    private int count = 0;
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    // To track which items have already been counted
    public HashSet<ItemChecker> countedCheckers = new HashSet<ItemChecker>();
    public HashSet<ItemCheckerChecker> countedItemCheckers = new HashSet<ItemCheckerChecker>();
    public HashSet<Keypad> countedKeypads = new HashSet<Keypad>();
    public HashSet<PatternChecker> countedPatterns = new HashSet<PatternChecker>();

    void Start()
    {
        if (GlobalSave.LoadedData != null)
        {
            // Load from global save if available
            PlayerSaveData data = GlobalSave.LoadedData;
            GlobalSave.LoadedData = null; // Clear after loading

            // Load the correct scene asynchronously
            SceneManager.LoadSceneAsync(data.sceneName).completed += op =>
            {
                Player player = FindFirstObjectByType<Player>();
                player.Sanity = data.sanity;
                player.Inventory = new List<string>(data.inventory);
                player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
                player.transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);

                // Restore global/scene states
                countedItemCheckers = data.completedItemCheckers;
                countedKeypads = data.completedKeypads;
                countedPatterns = data.completedPatternCheckers;
                countedCheckers = data.completedCheckers;

                for (int i = 0; i < countedItemCheckers.Count; i++)
                {
                    itemCheckerCheckers[i].AllItemsChecked = true;
                }
                for (int i = 0; i < countedKeypads.Count; i++)
                {
                    keypads[i].Completed = true;
                }
                for (int i = 0; i < countedPatterns.Count; i++)
                {
                    patternCheckers[i].Completed = true;
                }
                for (int i = 0; i < countedCheckers.Count; i++)
                {
                    checkers[i].HasSucceeded = true;
                }
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (groundDoorKey.HasSucceeded)
        {
            SceneManager.LoadScene("GroundFloor");
        }
        // Count each checker only once
        foreach (ItemChecker checker in checkers)
        {
            if (checker.HasSucceeded && !countedCheckers.Contains(checker))
            {
                countedCheckers.Add(checker);
            }
        }
        // Count each itemCheckerChecker only once
        foreach (ItemCheckerChecker icc in itemCheckerCheckers)
        {
            if (icc.AllItemsChecked && !countedItemCheckers.Contains(icc))
            {
                countedItemCheckers.Add(icc);
            }
        }
        // Count each keypad only once
        foreach (Keypad kp in keypads)
        {
            if (kp.Completed && !countedKeypads.Contains(kp))
            {
                countedKeypads.Add(kp);
            }
        }
        // Count each pattern checker only once
        foreach (PatternChecker pc in patternCheckers)
        {
            if (pc.Completed && !countedPatterns.Contains(pc))
            {
                countedPatterns.Add(pc);
            }
        }
        count = countedCheckers.Count + countedItemCheckers.Count + countedKeypads.Count + countedPatterns.Count;
        objectivesText.text = goals[count];
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
            completedItemCheckers = gm.countedItemCheckers,
            completedKeypads = gm.countedKeypads,
            completedPatternCheckers = gm.countedPatterns,
            completedCheckers = gm.countedCheckers,
        };

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        Debug.Log("Saved to " + SavePath);
    }
}
