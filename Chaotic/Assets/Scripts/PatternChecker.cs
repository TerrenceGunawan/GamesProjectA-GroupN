using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro; 

public class PatternChecker : MonoBehaviour, IInteractable
{
    private int correct;
    [SerializeField] private int correctCount = 4;
    [SerializeField] private Image[] toggleImages;

    public TextMeshProUGUI UIText;
    private int wrong;
    [SerializeField] private Player player;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject firstSelected;

    [SerializeField] private GameObject canvas;
    [SerializeField] private bool closeWithEscape = true;

    private bool isOpen;
    private bool Completed;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isOpen || panel == null || !panel.activeInHierarchy) return;

        if (player != null) player.DisableMovement();

        if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }
        else if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (EventSystem.current != null && firstSelected != null &&
                EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelected);
            }
        }

        if (closeWithEscape && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }

    void OnDisable()
    {
        if (isOpen) Close();
    }

    public void Correct(bool toggle)
    {
        if (toggle)
        {
            correct++;
            Complete();
        }
        else
        {
            correct--;
            Complete();
        }
    }

    public void Wrong(bool toggle)
    {
        if (toggle)
        {
            wrong++;
        }
        else
        {
            wrong--;
            Complete();
        }
    }

    private void Complete()
    {
        if (correct == correctCount && wrong == 0)
        {
            for (int i = 0; i < toggleImages.Length; i++)
            {
                toggleImages[i].color = new Color(0.3f, 1f, 0f);
            }
        }
        if (correct < 0)
        {
            correct = 0;
        }
    }

    public void Open()
    {
        if (isOpen || panel == null) return;
        isOpen = true;


        if (player != null)
        {
            player.SetPause = true;
            player.OnDisable();
            player.DisableMovement();
        }

        if (canvas != null)
        {
            canvas.SetActive(false);
        }


        if (crosshair != null) crosshair.SetActive(false);
        panel.SetActive(true);


        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            if (EventSystem.current != null && firstSelected != null)
                EventSystem.current.SetSelectedGameObject(firstSelected);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Close()
    {
        if (!isOpen || panel == null) return;
        isOpen = false;

        if (player != null)
        {
            player.SetPauseFunction();
            player.OnEnable();
        }

        if (canvas != null)
        {
            canvas.SetActive(true);
        }

        panel.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void Interact()
    {
        if (!isOpen)
        {
            Open(); 
        }    
    }

    public void OnRaycastHit()
    {
        if (!Completed)
        {
            UIText.text = "Interact [E]";
        }
    }
}
