using UnityEngine;
using UnityEngine.InputSystem;


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

    [SerializeField] private float maxDamageTimer = 2f;
    [SerializeField] private float damageTimer;
    [SerializeField] private float enemySanityDamage = 30f;

    [SerializeField] private float sanity = 100f;

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        rb.freezeRotation = true; // Prevent unwanted physics rotation
        actions = new PlayerActions();
        movementAction = actions.movement.walk;
        lookingAction = actions.movement.look;
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
        damageTimer = maxDamageTimer;
    }

    // Update is called once per frame
    void Update()
    {
        ReduceSanity();
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

    void ReduceSanity()
    {
        damageTimer -= Time.deltaTime;
        if(damageTimer < 0)
        {
            damageTimer = 0;
        }

        sanity -= Time.deltaTime;

        if(sanity < 0)
        {
            OnDisable();
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
        transform.position = hidingSpot.position;
        transform.rotation = hidingSpot.rotation;
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
}

