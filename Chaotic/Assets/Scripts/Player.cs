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
    
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float lookSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    private float verticalRotation = 0f;
    private bool isHidden = false;

    [SerializeField] private GameObject monster;
    private Vector3 monsterStartPosition;
    [SerializeField] private Slider sanityBar;
    [SerializeField] private float maxDamageTimer = 2f;
    [SerializeField] private float damageTimer;
    [SerializeField] private float enemySanityDamage = 30f;
    [SerializeField] private float hidingSanityMulti = 2f;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject crosshair;
    private float maxSanity;
    public float Sanity = 100f;
    public List<string> Inventory = new List<string>();
    private List<Transform> checkpoints = new List<Transform>();
    private GameObject[] items;

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
        items = GameObject.FindGameObjectsWithTag("Takeable");
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseLook();
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

        transform.Rotate(Vector3.up * lookInput.x * Time.deltaTime); // Rotate player horizontally

        verticalRotation -= lookInput.y * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0); // Rotate camera for vertical look
    }
    void OnCollisionEnter(Collision other)
    {
        if(damageTimer == 0)
        { 
        if (other.gameObject == monster)
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
        else
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

        if(IsHidden())
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
            Sanity += Time.deltaTime;
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
        isHidden = true;
        DisableMovement();

        rb.isKinematic = true; // Prevent physics from interfering
        if (hidingSpot.position.y < 0f)
        {
            transform.position = hidingSpot.position - new Vector3(0f, 1.0f, 0f);
        } 
        else
        {
            transform.position = hidingSpot.position;
        }
        transform.rotation = hidingSpot.rotation;

        verticalRotation = 0f;
        camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void ExitHiding(Vector3 exitOffset)
    {
        isHidden = false;
        EnableMovement();

        rb.isKinematic = false;
        transform.position += exitOffset;
    }

    public bool IsHidden()
    {
        return isHidden;
    }

    public void AddInventory(string itemName)
    {
        Inventory.Add(itemName);
    }

    void Restart()
    {
        // Sanity = maxSanity;
        // Inventory.Clear();
        // foreach (GameObject item in items)
        // {
        //     item.SetActive(true);
        // }
        // gameObject.transform.position = checkpoints[checkpoints.Count - 1].position;
        // monster.transform.position = monsterStartPosition;
        // OnEnable();
        // crosshair.SetActive(true);
        // Cursor.lockState = CursorLockMode.Locked;
        // restartButton.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);    
        }
}

