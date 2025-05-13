using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Keypad keypad;
    [SerializeField] private TextMeshProUGUI interactText;
    private bool done = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (keypad != null && keypad.Completed && !done)
        {
            done = true;
            StartCoroutine(Timer(true, 1f));
            StartCoroutine(Timer(false, 2.5f));
        }
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
