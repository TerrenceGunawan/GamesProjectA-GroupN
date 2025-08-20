using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
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
    private InputAction interactAction;
    private InputAction grabAction;
    private InputAction pauseAction;
    private Rigidbody rb;
    private AudioSource footstepSound;

    [SerializeField] private GameObject monster;
    [SerializeField] private Enemy enemy;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float mouseLookSensitivity = 10f;
    [SerializeField] private float controllerLookSensitivity = 200f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private float raycastDistance = 3f;
    private float verticalRotation = 0f;
    private Vector3 beforeHidingPosition;
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
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private Transform holdPoint; // empty GameObject in front of camera
    [SerializeField] private float grabForce = 200f;
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
    private ItemInteract grabbedItem = null;

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        rb.freezeRotation = true; // Prevent unwanted physics rotation
        actions = new PlayerActions();
        movementAction = actions.movement.walk;
        lookingAction = actions.movement.look;
        interactAction = actions.interaction.interact;
        grabAction = actions.interaction.grab;
        pauseAction = actions.interaction.pause;
        maxSanity = Sanity;
        footstepSound = GetComponent<AudioSource>();
    }

    public void OnEnable()
    {
        movementAction.Enable();
        lookingAction.Enable();
        interactAction.Enable();
        grabAction.Enable();
        pauseAction.Enable();
    }

    public void OnDisable()
    {
        movementAction.Disable();
        lookingAction.Disable();
        interactAction.Disable();
        grabAction.Disable();
        pauseAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Hide and lock cursor
        monsterStartPosition = monster.transform.position;
        items = FindObjectsByType<ItemInteract>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.GetComponentInParent<ItemInteract>() is ItemInteract movable)
            {
                if (hit.collider.gameObject != lastInteractedObject)
                {
                    movable.OnRaycastHit();
                    lastInteractedObject = hit.collider.gameObject;
                }

                // If grab button is held AND nothing grabbed → pick up
                if (movable.Movable && grabAction.IsPressed() && grabbedItem == null)
                {
                    interactText.text = "";
                    grabbedItem = movable;
                    Rigidbody grb = grabbedItem.GetComponent<Rigidbody>();
                    if (grb != null)
                    {
                        grb.useGravity = false;
                        grb.linearVelocity = Vector3.zero;
                        holdPoint.position = grabbedItem.transform.position;
                    }
                }

                // If grab button is released → drop
                if (!grabAction.IsPressed() && grabbedItem != null)
                {
                    DropItem();
                }
            }
            else if (hit.collider.GetComponentInParent<IInteractable>() is IInteractable interactable)
            {
                if (hit.collider.gameObject != lastInteractedObject)
                {
                    interactable.OnRaycastHit();
                    lastInteractedObject = hit.collider.gameObject;
                }

                if (interactAction.triggered)
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
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
        HandleMouseLook();
        float sanityPercent = Mathf.Clamp01(Sanity / maxSanity);
        sanityBar.fillAmount = sanityPercent;
        sanityBar.color = Color.Lerp(new Color(0.5f, 0, 0.5f), Color.white, sanityPercent);
        ReduceSanity();
        if (pauseAction.triggered && !SetPause)
        {
            if (isPaused && !SetPause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void FixedUpdate() // Use FixedUpdate for physics-based movement
    {
        HandleMovement();

        if (grabbedItem != null)
        {
            Rigidbody rb = grabbedItem.GetComponent<Rigidbody>();

            Vector3 targetPos = holdPoint.position;
            Vector3 moveDir = targetPos - rb.position;

            rb.linearVelocity = moveDir * grabForce * Time.fixedDeltaTime;
        }
    }

    void HandleMovement()
    {
        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        Vector3 moveDirection = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
        moveDirection.y = 0f; // Ensure no unintended vertical movement

        // Move the player while respecting colliders
        rb.MovePosition(rb.position + moveDirection * walkSpeed * Time.fixedDeltaTime);
        if (moveInput.magnitude > 0)
        {
            if (!footstepSound.isPlaying)
                footstepSound.Play();

            wasMoving = true;
        }
        else
        {
            if (wasMoving && footstepSound.isPlaying)
            {
                footstepSound.Stop();
                wasMoving = false;
            }
        }
    }

    void HandleMouseLook()
    {
        Vector2 lookInput = lookingAction.ReadValue<Vector2>();

        // Detect if input is coming from a gamepad or mouse
        float sensitivity = mouseLookSensitivity; // default

        if (lookingAction != null && lookingAction.activeControl != null)
        {
            if (lookingAction.activeControl.device is Gamepad)
                sensitivity = controllerLookSensitivity;
            else
                sensitivity = mouseLookSensitivity;
        }

        // Always allow horizontal look
        transform.Rotate(Vector3.up * lookInput.x * sensitivity * Time.deltaTime);

        verticalRotation -= lookInput.y * sensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (timer < 0)
            {
                Sanity -= enemySanityDamage;
                timerStart = false;
            }
            SetPauseFunction();
            enemy.TeleportToFurthestPatrolPoint();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SafeRoom")
        {
            RegainSanity();
        }
    }

    void DropItem()
    {
        if (grabbedItem != null)
        {
            Rigidbody rb = grabbedItem.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            grabbedItem = null;
        }
    }

    void ReduceSanity()
    {
        if (enemy.CanSeePlayer)
        {
            Sanity -= Time.deltaTime;
        }
        if (IsHidden)
        {
            Sanity -= hidingSanityMulti * Time.deltaTime;
        }
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
        if (Sanity >= maxSanity)
        {
            Sanity = maxSanity;
        }
        else
        {
            Sanity += sanityRegained;
        }
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
        rb.isKinematic = true; // Prevent physics from interfering
        verticalRotation = 0f;
        camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        OnDisable();
        transform.position = hidingSpot.position;
        transform.rotation = hidingSpot.rotation;
    }

    public void ExitHiding()
    {
        IsHidden = false;
        OnEnable();

        rb.isKinematic = false;
        transform.position = beforeHidingPosition;
    }

    public void AddInventory(string itemName)
    {
        Inventory.Add(itemName);
    }

    void PauseGame()
    {
        footstepSound.Stop();
        pauseMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        // If gamepad was used → auto-select first UI button + hide cursor
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

        Time.timeScale = 0f;
        isPaused = true;
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

