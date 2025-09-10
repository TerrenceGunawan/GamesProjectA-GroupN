using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private ItemChecker groundDoorKey;
    [SerializeField] private GameObject panel;
    [SerializeField] private List<string> goals = new List<string>();
    [SerializeField] private List<ItemChecker> checkers = new List<ItemChecker>();
    [SerializeField] private List<ItemCheckerChecker> itemCheckerCheckers = new List<ItemCheckerChecker>();
    [SerializeField] private List<Keypad> keypads = new List<Keypad>();
    [SerializeField] private List<PatternChecker> patternCheckers = new List<PatternChecker>();
    private bool done = false;
    private int count = 0;

    // To track which items have already been counted
    private HashSet<ItemChecker> countedCheckers = new HashSet<ItemChecker>();
    private HashSet<ItemCheckerChecker> countedItemCheckers = new HashSet<ItemCheckerChecker>();
    private HashSet<Keypad> countedKeypads = new HashSet<Keypad>();
    private HashSet<PatternChecker> countedPatterns = new HashSet<PatternChecker>();

    // Update is called once per frame
    void Update()
    {
        if (groundDoorKey.HasSucceeded)
        {
            //SceneManager.LoadScene("GroundFloor");
            panel.SetActive(true);
            EndGame();
        }
        // Count each checker only once
        foreach (ItemChecker checker in checkers)
        {
            if (checker.HasSucceeded && !countedCheckers.Contains(checker))
            {
                countedCheckers.Add(checker);
                count++;
            }
        }
        // Count each itemCheckerChecker only once
        foreach (ItemCheckerChecker icc in itemCheckerCheckers)
        {
            if (icc.AllItemsChecked && !countedItemCheckers.Contains(icc))
            {
                countedItemCheckers.Add(icc);
                count++;
            }
        }
        // Count each keypad only once
        foreach (Keypad kp in keypads)
        {
            if (kp.Completed && !countedKeypads.Contains(kp))
            {
                countedKeypads.Add(kp);
                count++;
            }
        }
        // Count each pattern checker only once
        foreach (PatternChecker pc in patternCheckers)
        {
            if (pc.Completed && !countedPatterns.Contains(pc))
            {
                countedPatterns.Add(pc);
                count++;
            }
        }
        objectivesText.text = goals[count];
    }
    
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}
