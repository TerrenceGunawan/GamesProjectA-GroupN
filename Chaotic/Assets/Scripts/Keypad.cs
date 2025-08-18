using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro; 


public class Keypad : MonoBehaviour, IInteractable
{
    public Player player;
    public GameObject crosshair;
    public GameObject keypadOB;
    public List<GameObject> rewards = new List<GameObject>();
    public GameObject firstSelected;
    public Animator ANI;

    public TextMeshProUGUI keypadText;
    public TextMeshProUGUI textOB;
    public string answer = "12345";

    public AudioClip button;
    public AudioClip correct;
    public AudioClip wrong;
    private AudioSource audioSource;

    public bool animate;
    public bool Completed = false;
    public bool rewardCheck = false;


    void Start()
    {
        keypadOB.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void OpenKeypadUI()
    {
        player.SetPause = true;
        crosshair.SetActive(false);
        keypadOB.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        // If gamepad is active → auto-select the first button and hide cursor
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else // If mouse/keyboard → show cursor
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        player.OnDisable();
    }

    public void Number(int number)
    {
        textOB.text += number.ToString();
        audioSource.clip = button;
        audioSource.Play();
    }

    public void Enter()
    {
        if (textOB.text == answer)
        {
            if (rewardCheck)
            {
                animate = true;
                foreach (GameObject reward in rewards)
                {
                    reward.SetActive(true);
                }
            }
            audioSource.clip = correct;
            audioSource.Play();
            Completed = true;
            textOB.text = "Correct";
            StartCoroutine(CodeDelay(true, 0.5f));
        }
        else
        {
            audioSource.clip = wrong;
            audioSource.Play();
            textOB.text = "X";
            StartCoroutine(CodeDelay(false, 0.5f));
        }


    }

    public void Clear()
    {
        {
            textOB.text = "";
            audioSource.clip = button;
            audioSource.Play();
        }
    }

    public void Exit()
    {
        if (!Completed)
        {
            keypadText.text = "Interact";
        }
        player.SetPauseFunction();
        keypadOB.SetActive(false);
        crosshair.SetActive(true);  // Show the crosshair again
        player.OnEnable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Interact()
    {
         // If the player presses the interact button and is within reach
        if (!Completed)
        {
            OpenKeypadUI();  // Call method to open the keypad UI
            keypadText.text = "";
        }

        if (Input.GetKeyDown(KeyCode.Escape) && keypadOB.activeInHierarchy)
        {
            Exit();
        }
    }

    public void OnRaycastHit()
    {
        if (!Completed)
        {
            keypadText.text = "Interact";
        }
    }

    public void Update()
    {
        if (animate)
        {
            ANI.SetBool("animate", true);
        }
        if (keypadOB.activeInHierarchy)
        {
            player.DisableMovement();

            // Dynamically check input device
            if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            {
                // Mouse moved → show cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                // Gamepad used → hide cursor
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
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