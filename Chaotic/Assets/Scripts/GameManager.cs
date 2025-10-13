using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject saveIcon;
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private ItemChecker groundDoorKey;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private List<string> goals = new List<string>();
    [SerializeField] private List<ItemChecker> checkers = new List<ItemChecker>();
    [SerializeField] private List<Keypad> keypads = new List<Keypad>();    
    [SerializeField] private List<PatternChecker> patternCheckers = new List<PatternChecker>();
    private bool done = false;
    private int count = 0;

    // To track which items have already been counted
    private HashSet<ItemChecker> countedCheckers = new HashSet<ItemChecker>();
    private HashSet<Keypad> countedKeypads = new HashSet<Keypad>();
    private HashSet<PatternChecker> countedPatterns = new HashSet<PatternChecker>();

    // Update is called once per frame
    void Update()
    {
        if (groundDoorKey.HasSucceeded)
        {
            endPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // Count each checker only once
        foreach (ItemChecker checker in checkers)
        {
            if (checker.HasSucceeded && !countedCheckers.Contains(checker))
            {
                countedCheckers.Add(checker);
<<<<<<< Updated upstream
                count++;
=======
                StartCoroutine(SaveIcon());
            }
        }
        // Count each itemCheckerChecker only once
        foreach (ItemCheckerChecker icc in itemCheckerCheckers)
        {
            if (icc.AllItemsChecked && !countedItemCheckers.Contains(icc))
            {
                countedItemCheckers.Add(icc);
                StartCoroutine(SaveIcon());
>>>>>>> Stashed changes
            }
        }
        // Count each keypad only once
        foreach (Keypad kp in keypads)
        {
            if (kp.Completed && !countedKeypads.Contains(kp))
            {
                countedKeypads.Add(kp);
<<<<<<< Updated upstream
                count++;
=======
                StartCoroutine(SaveIcon());
>>>>>>> Stashed changes
            }
        }
        // Count each pattern checker only once
        foreach (PatternChecker pc in patternCheckers)
        {
            if (pc.Completed && !countedPatterns.Contains(pc))
            {
                countedPatterns.Add(pc);
<<<<<<< Updated upstream
                count++;
=======
                StartCoroutine(SaveIcon());
>>>>>>> Stashed changes
            }
        }
        objectivesText.text = goals[count];
    }
<<<<<<< Updated upstream
=======

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

    private IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Ending");
    }

    private IEnumerator SaveIcon()
    {
        SaveGame();
        saveIcon.SetActive(true);
        yield return new WaitForSeconds(2f);
        saveIcon.SetActive(false);
    }
>>>>>>> Stashed changes
}
