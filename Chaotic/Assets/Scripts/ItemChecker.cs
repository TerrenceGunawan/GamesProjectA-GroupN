using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemChecker : MonoBehaviour
{
    [SerializeField] private List<string> itemsNeeded = new List<string>();
    [SerializeField] private Player player;
    [SerializeField] private GameObject interactText;
    [SerializeField] private GameObject successText;
    [SerializeField] private GameObject failText;
    private bool inReach = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E) && Check(itemsNeeded, player.Inventory))
        {
            interactText.SetActive(false);
            successText.SetActive(true);
            StartCoroutine(HideTextAfterSeconds(successText, 3f));
        }
        else if (inReach && Input.GetKeyDown(KeyCode.E) && !Check(itemsNeeded, player.Inventory))
        {
            interactText.SetActive(false);
            failText.SetActive(true);
            StartCoroutine(HideTextAfterSeconds(failText, 3f));
        }
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

    bool Check(List<string> list1, List<string> list2)
    {
        return new HashSet<string>(list1).SetEquals(list2);
    }

    IEnumerator HideTextAfterSeconds(GameObject text, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.SetActive(false);
    }
}
