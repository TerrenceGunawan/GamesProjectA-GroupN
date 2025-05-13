using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class Keypad : MonoBehaviour
{
    public Player player;
    public GameObject crosshair;
    public GameObject keypadOB;
    public GameObject animateOB;
    public Animator ANI;

    public TextMeshProUGUI keypadText;
    public TextMeshProUGUI textOB;
    public string answer = "12345";
    public bool Right;

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool inReach = false;
    public bool animate;
    private bool completed = false;


    void Start()
    {
        keypadOB.SetActive(false);
        Right = false;
    }

    void OpenKeypadUI()
    {
        crosshair.SetActive(false);  // Hide the crosshair when interacting
        keypadOB.SetActive(true);  // Show the keypad UI
        player.DisableMovement();  // Disable player movement when interacting
    }

    public void Number(int number)
    {
        textOB.text += number.ToString();
        Right = true;
        // button.Play();
    }

    public void Enter()
    {
        if (textOB.text == answer)
        {
            // correct.Play();
            completed = true;
            textOB.text = "Correct";
            StartCoroutine(CodeDelay(true, 0.5f));
        }
        else
        {
            // wrong.Play();
            textOB.text = "X";
            StartCoroutine(CodeDelay(false, 0.5f));
        }


    }

    public void Clear()
    {
        {
            textOB.text = "";
            button.Play();
        }
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        crosshair.SetActive(true);  // Show the crosshair again
        player.EnableMovement();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        // If the player presses the interact button and is within reach
        if (Input.GetKeyDown(KeyCode.E) && inReach && !completed) 
        {
            OpenKeypadUI();  // Call method to open the keypad UI
            keypadText.text = "";
        }

        if (textOB.text == "Right" && animate)
        {
            ANI.SetBool("animate", true);
            Debug.Log("its open");
        }


        if(keypadOB.activeInHierarchy)
        {
            player.DisableMovement();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Reach") && !completed)  // When the player enters the trigger
        {
            inReach = true;
            keypadText.text = "Interact [E]";  // Show interaction text
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Reach"))  // When the player exits the trigger
        {
            inReach = false;
            keypadText.text = "";  // Hide interaction text
        }
    }

    IEnumerator CodeDelay(bool correct, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (correct)
        {
            Exit();
        }
        else
        {
            textOB.text = "";
        }
    }
}