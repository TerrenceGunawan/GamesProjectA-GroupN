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
    
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float lookSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    private float verticalRotation = 0f;
    private bool isHidden = false;

    [SerializeField] private Slider sanityBar;
    [SerializeField] private float maxDamageTimer = 2f;
    [SerializeField] private float damageTimer;
    [SerializeField] private float enemySanityDamage = 30f;
    [SerializeField] private float hidingSanityMulti = 2f;
    [SerializeField] private float sanity = 100f;
    [SerializeField] private Button restartButton;
    private float maxSanity;
    public List<string> Inventory = new List<string>();

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
        maxSanity = sanity;
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
        sanityBar.value = sanity;
        damageTimer = maxDamageTimer;
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
        if (other.gameObject.CompareTag("Enemy"))
        {
            sanity -=enemySanityDamage;
            damageTimer = maxDamageTimer;
            Debug.Log("You got hit");
        }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "DangerRoom")
        {
            ReduceSanity();
        }
        if (other.gameObject.tag == "SafeRoom")
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
            sanity -= hidingSanityMulti * Time.deltaTime;
        }
        else
        {
            sanity -= Time.deltaTime;
        }

        sanityBar.value = sanity;

        if(sanity < 0)
        {
            OnDisable();
            restartButton.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void RegainSanity()
    {
        if(sanity >= maxSanity)
        {
            sanity = maxSanity;
        }
        else
        {
            sanity += Time.deltaTime;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

