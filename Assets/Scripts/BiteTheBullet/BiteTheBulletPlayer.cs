using UnityEngine;
using UnityEngine.InputSystem;

namespace TakenLiterally.BiteTheBullet
{
    /// <summary>
    /// Simple first-person controller for the "Bite the Bullet" scene.
    /// Uses the New Input System that your project already has installed.
    /// Attach this to the Player GameObject.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class BiteTheBulletPlayer : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("How fast the player moves.")]
        [SerializeField] private float moveSpeed = 6f;

        [Tooltip("How high the player jumps.")]
        [SerializeField] private float jumpForce = 7f;

        [Header("Jump Feel")]
        [Tooltip("Seconds after leaving the ground where you can still jump.")]
        [SerializeField] private float coyoteTime = 0.15f;

        [Tooltip("Seconds before landing where a jump press is remembered.")]
        [SerializeField] private float jumpBufferTime = 0.2f;

        [Header("Camera")]
        [Tooltip("Mouse look sensitivity.")]
        [SerializeField] private float lookSensitivity = 2f;

        [Tooltip("Maximum up/down look angle.")]
        [SerializeField] private float maxLookAngle = 80f;

        // Internal state
        private Rigidbody rb;
        private Camera playerCamera;
        private float verticalRotation = 0f;

        // Jump state
        private float coyoteTimer;
        private float jumpBufferTimer;
        private bool isGrounded;

        // Input values
        private Vector2 moveInput;
        private Vector2 lookInput;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.mass = 2f;

            // Create a camera as a child of the player
            GameObject camObj = new GameObject("PlayerCamera");
            camObj.transform.SetParent(transform);
            camObj.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            playerCamera = camObj.AddComponent<Camera>();
            playerCamera.nearClipPlane = 0.1f;

            // Lock the cursor for first-person control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            // WASD movement
            moveInput = Vector2.zero;
            if (keyboard.wKey.isPressed) moveInput.y += 1f;
            if (keyboard.sKey.isPressed) moveInput.y -= 1f;
            if (keyboard.aKey.isPressed) moveInput.x -= 1f;
            if (keyboard.dKey.isPressed) moveInput.x += 1f;

            // Mouse look
            lookInput = mouse.delta.ReadValue() * lookSensitivity * 0.1f;

            // Jump — buffer the press so FixedUpdate never misses it
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                jumpBufferTimer = jumpBufferTime;
            }

            ApplyLook();

            // ESC to unlock cursor
            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        void FixedUpdate()
        {
            CheckGrounded();     // Check ground FIRST
            ApplyMovement();
            ApplyJump();

            // Count down timers
            if (coyoteTimer > 0f) coyoteTimer -= Time.fixedDeltaTime;
            if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.fixedDeltaTime;
        }

        private void ApplyLook()
        {
            transform.Rotate(Vector3.up * lookInput.x);
            verticalRotation -= lookInput.y;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

            if (playerCamera != null)
            {
                playerCamera.transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
            }
        }

        private void ApplyMovement()
        {
            Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;
            moveDir.y = 0f;
            moveDir = moveDir.normalized * moveSpeed;

            Vector3 velocity = rb.linearVelocity;
            rb.linearVelocity = new Vector3(moveDir.x, velocity.y, moveDir.z);
        }

        /// <summary>
        /// Jump if we have a buffered press AND we're grounded (or within coyote time).
        /// </summary>
        private void ApplyJump()
        {
            bool canJump = isGrounded || coyoteTimer > 0f;

            if (jumpBufferTimer > 0f && canJump)
            {
                // Zero out downward velocity so the jump feels consistent
                Vector3 vel = rb.linearVelocity;
                rb.linearVelocity = new Vector3(vel.x, 0f, vel.z);

                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // Consume both timers so we don't double-jump
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }
        }

        /// <summary>
        /// Check if the player is standing on something.
        /// Runs BEFORE jump so the result is always fresh.
        /// </summary>
        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;

            isGrounded = Physics.SphereCast(
                transform.position + Vector3.up * 0.1f,
                0.3f,
                Vector3.down,
                out _,
                0.9f
            );

            // If we just left the ground, start coyote timer
            if (wasGrounded && !isGrounded)
            {
                coyoteTimer = coyoteTime;
            }
        }
    }
}
