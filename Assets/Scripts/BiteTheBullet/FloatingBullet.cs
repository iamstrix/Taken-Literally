using UnityEngine;

namespace TakenLiterally.BiteTheBullet
{
    /// <summary>
    /// Makes a bullet float and drift unpredictably in 3D space.
    /// Attach this to any GameObject that should act as a "floating bullet."
    /// The bullet uses antigravity (upward force) combined with random drift
    /// to create an organic, hovering movement pattern.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class FloatingBullet : MonoBehaviour
    {
        [Header("Floating Settings")]
        [Tooltip("How strongly the bullet resists gravity (pushes upward).")]
        [SerializeField] private float antigravityForce = 12f;

        [Tooltip("The target height the bullet tries to hover at.")]
        [SerializeField] private float hoverHeight = 1.5f;

        [Tooltip("How strongly the bullet corrects itself toward the hover height.")]
        [SerializeField] private float hoverStiffness = 5f;

        [Header("Drift Settings")]
        [Tooltip("How fast the bullet drifts around randomly.")]
        [SerializeField] private float driftSpeed = 2f;

        [Tooltip("How often the bullet picks a new drift direction (seconds).")]
        [SerializeField] private float driftChangeInterval = 2f;

        [Tooltip("Maximum distance the bullet can drift from its spawn point.")]
        [SerializeField] private float maxDriftRadius = 8f;

        [Header("Bounce Settings")]
        [Tooltip("How bouncy the bullet is when it hits something.")]
        [SerializeField] private float bounciness = 0.5f;

        // Internal state
        private Rigidbody rb;
        private Vector3 spawnPoint;
        private Vector3 currentDriftDirection;
        private float driftTimer;

        // Unique random seed per bullet so they all move differently
        private float noiseOffset;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();

            // Configure the Rigidbody for floating behavior:
            // - No angular drag so it tumbles naturally
            // - Some linear drag so it doesn't fly away forever
            rb.useGravity = true;          // We apply our own "antigravity" on top
            rb.linearDamping = 1f;
            rb.angularDamping = 0.5f;
            rb.mass = 0.5f;

            // Remember where we spawned
            spawnPoint = transform.position;

            // Give each bullet a unique noise pattern
            noiseOffset = Random.Range(0f, 1000f);

            // Pick an initial drift direction
            PickNewDriftDirection();
        }

        void FixedUpdate()
        {
            ApplyAntigravity();
            ApplyHoverCorrection();
            ApplyDrift();
            ClampToRadius();
        }

        /// <summary>
        /// Push the bullet upward to counteract gravity, creating a "weightless" feel.
        /// </summary>
        private void ApplyAntigravity()
        {
            // Apply a force that is slightly stronger than gravity
            // Physics.gravity.y is typically -9.81, so we push upward
            rb.AddForce(Vector3.up * antigravityForce, ForceMode.Acceleration);
        }

        /// <summary>
        /// Gently push the bullet toward its target hover height,
        /// like an invisible spring pulling it to the right altitude.
        /// </summary>
        private void ApplyHoverCorrection()
        {
            float heightError = (spawnPoint.y + hoverHeight) - transform.position.y;
            float correctionForce = heightError * hoverStiffness;
            rb.AddForce(Vector3.up * correctionForce, ForceMode.Acceleration);
        }

        /// <summary>
        /// Push the bullet in a slowly-changing random direction
        /// so it wanders around unpredictably.
        /// </summary>
        private void ApplyDrift()
        {
            // Count down to pick a new direction
            driftTimer -= Time.fixedDeltaTime;
            if (driftTimer <= 0f)
            {
                PickNewDriftDirection();
            }

            // Use Perlin noise for smooth, organic movement
            float noiseX = Mathf.PerlinNoise(Time.time * 0.5f + noiseOffset, 0f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(0f, Time.time * 0.5f + noiseOffset) - 0.5f;
            Vector3 noiseDir = new Vector3(noiseX, 0f, noiseZ).normalized;

            // Combine the random direction with noise for organic feel
            Vector3 driftForce = (currentDriftDirection + noiseDir) * driftSpeed;
            rb.AddForce(driftForce, ForceMode.Acceleration);
        }

        /// <summary>
        /// If the bullet drifts too far from its spawn point,
        /// gently push it back toward the center.
        /// </summary>
        private void ClampToRadius()
        {
            Vector3 offset = transform.position - spawnPoint;
            offset.y = 0; // Only clamp horizontally

            if (offset.magnitude > maxDriftRadius)
            {
                Vector3 pullBack = -offset.normalized * 2f;
                rb.AddForce(pullBack, ForceMode.Acceleration);
            }
        }

        /// <summary>
        /// Pick a new random horizontal direction for the bullet to drift toward.
        /// </summary>
        private void PickNewDriftDirection()
        {
            currentDriftDirection = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;

            driftTimer = driftChangeInterval + Random.Range(-0.5f, 0.5f);
        }

        /// <summary>
        /// Optional: Make the bullet gently bounce off surfaces.
        /// Called automatically by Unity when this bullet hits something.
        /// </summary>
        void OnCollisionEnter(Collision collision)
        {
            // If we hit a wall or floor, bounce away
            if (!collision.gameObject.CompareTag("Player"))
            {
                Vector3 bounceDir = collision.contacts[0].normal;
                rb.AddForce(bounceDir * bounciness, ForceMode.Impulse);
            }
        }
    }
}
