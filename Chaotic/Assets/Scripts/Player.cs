using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class Player : MonoBehaviour
{
    private Camera camera;
    private PlayerActions actions;
    private InputAction movementAction;
    private InputAction lookingAction;
    private AudioSource footstepSound;

    [SerializeField] private GameObject monster;
    [SerializeField] private Enemy enemy;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float lookSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private float raycastDistance = 3f;
    private float verticalRotation = 0f;
    private Vector3 beforeHidingPosition;
    private bool limitVerticalLook = false;
    public bool IsHidden = false;

    private Vector3 monsterStartPosition;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Image sanityBar;
    [SerializeField] private float enemySanityDamage = 30f;
    [SerializeField] private float hidingSanityMulti = 2f;
    [SerializeField] private float sanityRegained = 20f;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject pauseMenu;

    private CharacterController controller;
    private Vector3 velocity;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    private bool isPaused;
    private bool wasMoving;
    private float maxSanity;
    public float Sanity = 100f;

    public bool SetPause;
    public bool timerStart;
    public float timer = 0.1f;
    public List<string> Inventory = new List<string>();
    private ItemInteract[] items;
    private GameObject lastInteractedObject = null;

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        actions = new PlayerActions();
        movementAction = actions.movement.walk;
        lookingAction = actions.movement.look;
        maxSanity = Sanity;
        footstepSound = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        movementAction.Enable();
        lookingAction.Enable();
    }

    void OnDisable()
    {
        movementAction.Disable();
        lookingAction.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        monsterStartPosition = monster.transform.position;
        items = FindObjectsByType<ItemInteract>(FindObjectsSortMode.None);
    }

    void Update()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.GetComponentInParent<IInteractable>() is IInteractable interactable)
            {
                if (hit.collider.gameObject != lastInteractedObject)
                {
                    interactable.OnRaycastHit();
                    lastInteractedObject = hit.collider.gameObject;
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
            else if (lastInteractedObject != null)
            {
                interactText.text = "";
                lastInteractedObject = null;
            }
        }
        else if (lastInteractedObject != null)
        {
            interactText.text = "";
            lastInteractedObject = null;
        }

        if (timerStart)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                SetPause = false;
                timerStart = false;
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);
        HandleMouseLook();
        HandleMovement();

        float sanityPercent = Mathf.Clamp01(Sanity / maxSanity);
        sanityBar.fillAmount = sanityPercent;
        sanityBar.color = Color.Lerp(new Color(0.5f, 0, 0.5f), Color.white, sanityPercent);
        ReduceSanity();

        if (Input.GetKeyDown(KeyCode.Escape) && !SetPause)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * walkSpeed * Time.deltaTime);

        if (moveInput.magnitude > 0)
        {
            if (!footstepSound.isPlaying)
                footstepSound.Play();

            wasMoving = true;
        }
        else if (wasMoving && footstepSound.isPlaying)
        {
            footstepSound.Stop();
            wasMoving = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        Vector2 lookInput = lookingAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(Vector3.up * lookInput.x * Time.deltaTime);

        if (!limitVerticalLook)
        {
            verticalRotation -= lookInput.y * Time.deltaTime;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
        else
        {
            camera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("SafeRoom"))
        {
            RegainSanity();
        }
    }

    void ReduceSanity()
    {
        if (enemy.CanSeePlayer)
            Sanity -= Time.deltaTime;

        if (IsHidden)
            Sanity -= hidingSanityMulti * Time.deltaTime;

        if (Sanity < 0)
        {
            OnDisable();
            Time.timeScale = 0f;
            crosshair.SetActive(false);
            gameOver.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void RegainSanity()
    {
        Sanity = Mathf.Min(Sanity + sanityRegained, maxSanity);
    }

    public void EnableMovement()
    {
        movementAction.Enable();
        lookingAction.Enable();
    }

    public void DisableMovement()
    {
        movementAction.Disable();
        lookingAction.Disable();
    }

    public void HideAtPosition(Transform hidingSpot)
    {
        IsHidden = true;
        beforeHidingPosition = transform.position;
        velocity = Vector3.zero;
        verticalRotation = 0f;
        camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        DisableMovement();
        controller.enabled = false;
        transform.position = hidingSpot.position;
        transform.rotation = hidingSpot.rotation;
    }

    public void ExitHiding()
    {
        limitVerticalLook = false;
        IsHidden = false;
        controller.enabled = true;
        transform.position = beforeHidingPosition;
        EnableMovement();
    }

    public void AddInventory(string itemName)
    {
        Inventory.Add(itemName);
    }

    void PauseGame()
    {
        footstepSound.Stop();
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetPause = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void SetPauseFunction()
    {
        timer = 0.1f;
        timerStart = true;
    }
}
