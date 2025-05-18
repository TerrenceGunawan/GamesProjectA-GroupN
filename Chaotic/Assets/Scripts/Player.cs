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
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float lookSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    private float verticalRotation = 0f;
    private Vector3 beforeHidingPosition;
    private bool limitVerticalLook = false;
    public bool IsHidden = false;

    private Vector3 monsterStartPosition;
    [SerializeField] private RawImage sanityOverlay;
    [SerializeField] private Slider sanityBar;
    [SerializeField] private float maxDamageTimer = 1f;
    [SerializeField] private float damageTimer;
    [SerializeField] private float enemySanityDamage = 30f;
    [SerializeField] private float hidingSanityMulti = 2f;
    [SerializeField] private float sanityRegained = 20f;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject crosshair;
    private float maxSanity;
    public float Sanity = 100f;
    public List<string> Inventory = new List<string>();
    private List<Transform> checkpoints = new List<Transform>();
    private ItemInteract[] items;

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        rb.freezeRotation = true; // Prevent unwanted physics rotation
        actions = new PlayerActions();
        movementAction = actions.movement.walk;
        lookingAction = actions.movement.look;
        restartButton.onClick.AddListener(Restart);
        restartButton.gameObject.SetActive(false);
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
        sanityBar.value = Sanity;
        damageTimer = maxDamageTimer;
        monsterStartPosition = monster.transform.position;
        items = FindObjectsByType<ItemInteract>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseLook();
        float darkness = 1f - (Sanity / maxSanity); // 0 (normal) to 1 (fully insane)
        Color overlayColor = sanityOverlay.color;
        if (IsHidden)
        {
            overlayColor.a = 0.6f; // No overlay when hidden
        }
        else
        {
            overlayColor.a = Mathf.Lerp(0f, 0.2f, darkness); // max 70% opacity
        }
        sanityOverlay.color = overlayColor;
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
        footstepSound.enabled = moveInput.magnitude > 0; // Enable footstep sound only when moving
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
        if(damageTimer == 0)
        { 
        if (other.gameObject.tag == "Enemy")
        {
            Sanity -= enemySanityDamage;
            damageTimer = maxDamageTimer;
            Debug.Log("You got hit");
        }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            checkpoints.Add(other.gameObject.transform);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "DangerRoom")
        {
            ReduceSanity();
        }
        else if (other.gameObject.tag == "SafeRoom")
        {
            RegainSanity();
        }
    }

    void ReduceSanity()
    {
        damageTimer -= Time.deltaTime;
        if(damageTimer < 0)
        {
            damageTimer = 0;
        }

        if(IsHidden)
        {
            Sanity -= hidingSanityMulti * Time.deltaTime;
        }
        else
        {
            Sanity -= Time.deltaTime;
        }

        sanityBar.value = Sanity;

        if(Sanity < 0)
        {
            OnDisable();
            crosshair.SetActive(false);
            restartButton.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void RegainSanity()
    {
        if(Sanity >= maxSanity)
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
        Debug.Log("You got " + itemName);
    }

    void Restart()
    {
        rb.isKinematic = false;
        Sanity = maxSanity;
        foreach (ItemInteract item in items)
        {
            item.gameObject.SetActive(true);
            item.Taken = false;
        }
        Inventory.Clear();
        gameObject.transform.position = checkpoints[checkpoints.Count - 1].position;
        monster.transform.position = monsterStartPosition;
        OnEnable();
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        restartButton.gameObject.SetActive(false);
        }
}

