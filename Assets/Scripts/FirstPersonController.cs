using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float lookSensitivity = 0.2f;
    public Transform playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Attempt to automatically assign the camera if it's a child object
        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerCamera = cam.transform;
            }
            else
            {
                Debug.LogWarning("No camera found. Please make the Main Camera a child of this object or assign it to the Player Camera field in the inspector.");
            }
        }
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // Check if the player is grounded
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small constant downward force to keep player glued to the ground
        }

        // Get WASD input using the New Input System directly
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
        }

        // Normalize movement vector so diagonal movement isn't faster
        Vector3 moveInput = new Vector3(x, 0, z).normalized;

        // Calculate move direction relative to the player's facing direction
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.z;
        characterController.Move(move * walkSpeed * Time.deltaTime);

        // Handle Jump
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null) return;

        // Get Mouse delta using the New Input System
        float mouseX = 0f;
        float mouseY = 0f;

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            mouseX = delta.x * lookSensitivity;
            mouseY = delta.y * lookSensitivity;
        }

        // Calculate vertical rotation and clamp it so you can't look past straight up or down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply vertical rotation to the camera
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation to the player body
        transform.Rotate(Vector3.up * mouseX);
    }
}
