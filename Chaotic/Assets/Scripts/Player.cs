using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    private Camera camera;
    private PlayerActions actions;
    private InputAction movementAction;
    private InputAction lookingAction;
    
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float lookSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    private float verticalRotation = 0f;

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
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
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        Vector2 moveInput = movementAction.ReadValue<Vector2>();

        // Convert 2D input to 3D world movement
        Vector3 moveDirection = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
        
        // Apply movement (ignoring Y to prevent floating)
        moveDirection.y = 0f;
        
        transform.position += moveDirection * walkSpeed * Time.deltaTime; // Move the player
    }

    void HandleMouseLook()
    {
        Vector2 lookInput = lookingAction.ReadValue<Vector2>() * lookSensitivity;

        transform.Rotate(Vector3.up * lookInput.x * Time.deltaTime); // Rotate player horizontally

        verticalRotation -= lookInput.y * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0); // Rotate camera for vertical look
    }
}
