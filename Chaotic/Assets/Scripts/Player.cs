using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    private Camera camera;
    private PlayerActions actions;
    private InputAction movementAction;
    private InputAction lookingAction;
    private Rigidbody rb;
    private AudioSource footstepSound;

    [SerializeField] private GameObject monster;
    [SerializeField] private Enemy enemy;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float lookSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    private float verticalRotation = 0f;
    private Vector3 beforeHidingPosition;
    private bool limitVerticalLook = false;
    public bool IsHidden = false;

    private Vector3 monsterStartPosition;
    [SerializeField] private Image sanityBar;
    [SerializeField] private float enemySanityDamage = 30f;
    [SerializeField] private float hidingSanityMulti = 2f;
    [SerializeField] private float sanityRegained = 20f;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused;
    private bool wasMoving;
    private float maxSanity;
    public float Sanity = 100f;

    public bool SetPause;
    public bool timerStart;
    public float timer = 0.1f;
    public List<string> Inventory = new List<string>();
    private ItemInteract[] items;
   

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        rb.freezeRotation = true; // Prevent unwanted physics rotation
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
        if (timerStart)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                SetPause = false;
                timerStart = false;
            }
        }
        HandleMouseLook();
        float sanityPercent = Mathf.Clamp01(Sanity / maxSanity);
        sanityBar.fillAmount = sanityPercent;
        sanityBar.color = Color.Lerp(new Color(0.5f, 0, 0.5f), Color.white, sanityPercent);
        ReduceSanity();
        if (Input.GetKeyDown(KeyCode.Escape) && !SetPause)
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
    }

    void HandleMovement()
    {
        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        Vector3 moveDirection = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
        moveDirection.y = 0f; // Ensure no unintended vertical movement

        // Move the player while respecting colliders
        rb.MovePosition(rb.position + moveDirection * walkSpeed * Time.fixedDeltaTime);
       // footstepSound.enabled = moveInput.magnitude > 0; // Enable footstep sound only when moving
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
        Vector2 lookInput = lookingAction.ReadValue<Vector2>() * lookSensitivity;

        // Always allow horizontal look
        transform.Rotate(Vector3.up * lookInput.x * Time.deltaTime);

        // Only allow vertical look if not limited
        if (!limitVerticalLook)
        {
            verticalRotation -= lookInput.y * Time.deltaTime;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
        else
        {
            camera.transform.localRotation = Quaternion.Euler(0, 0, 0); // Lock to forward
        }
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
        if (hidingSpot.position.y < 1f)
        {
            limitVerticalLook = true;
            movementAction.Disable();
            transform.position = hidingSpot.position - new Vector3(0f, 1f, 0f);
        }
        else
        {
            OnDisable();
            transform.position = hidingSpot.position;
            transform.rotation = hidingSpot.rotation;
        }
    }

    public void ExitHiding()
    {
        limitVerticalLook = false;
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

