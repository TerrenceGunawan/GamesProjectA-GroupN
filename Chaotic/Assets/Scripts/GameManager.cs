using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Keypad keypad;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private ItemChecker groundDoorKey;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private List<string> goals = new List<string>();
    [SerializeField] private List<ItemChecker> checkers = new List<ItemChecker>();
    [SerializeField] private List<Keypad> keypads = new List<Keypad>();    
    private bool done = false;
    private int count = 0;

    // To track which items have already been counted
    private HashSet<ItemChecker> countedCheckers = new HashSet<ItemChecker>();
    private HashSet<Keypad> countedKeypads = new HashSet<Keypad>();

    // Update is called once per frame
    void Update()
    {
        if (keypad != null && keypad.Completed && !done)
        {
            done = true;
            StartCoroutine(Timer(true, 1f));
            StartCoroutine(Timer(false, 2.5f));
        }
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
        objectivesText.text = goals[count];
    }

    IEnumerator Timer(bool started, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (started)
        {
            interactText.text = "Press [F] to use Flashlight";
        }
        else
        {
            interactText.text = "";
        }
    }
}
