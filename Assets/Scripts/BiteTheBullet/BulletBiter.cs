using UnityEngine;

namespace TakenLiterally.BiteTheBullet
{
    /// <summary>
    /// Handles the player "biting" (colliding with) bullets.
    /// Attach this to the Player GameObject.
    /// When the player intentionally moves into a bullet, it counts as a "bite."
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BulletBiter : MonoBehaviour
    {
        [Header("Score")]
        [Tooltip("How many bullets the player has bitten so far.")]
        [SerializeField] private int bulletsBitten = 0;

        [Header("Visual Feedback")]
        [Tooltip("How long the player flashes when they bite a bullet.")]
        [SerializeField] private float flashDuration = 0.3f;

        [Tooltip("The color the player flashes when biting a bullet.")]
        [SerializeField] private Color flashColor = Color.yellow;

        // Internal references
        private Renderer playerRenderer;
        private Color originalColor;
        private float flashTimer;
        private bool isFlashing;

        // Reference to the spawner so we can tell it a bullet was destroyed
        private BulletSpawner spawner;

        void Start()
        {
            // Get the player's renderer for visual feedback
            playerRenderer = GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                originalColor = playerRenderer.material.color;
            }

            // Find the spawner in the scene
            spawner = Object.FindAnyObjectByType<BulletSpawner>();

            // Make sure this GameObject is tagged as "Player"
            gameObject.tag = "Player";

            Debug.Log("=== BITE THE BULLET ===");
            Debug.Log("Move into the floating bullets to 'bite' them!");
            Debug.Log("Use WASD to move, SPACE to jump.");
        }

        void Update()
        {
            // Handle the flash effect countdown
            if (isFlashing)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0f)
                {
                    isFlashing = false;
                    if (playerRenderer != null)
                    {
                        playerRenderer.material.color = originalColor;
                    }
                }
            }
        }

        /// <summary>
        /// Called automatically by Unity when this object's collider
        /// touches another collider. We check if it's a bullet.
        /// </summary>
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                BiteBullet(collision.gameObject);
            }
        }

        /// <summary>
        /// Also check trigger colliders (in case bullets use triggers).
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                BiteBullet(other.gameObject);
            }
        }

        /// <summary>
        /// The player has intentionally collided with a bullet — they "bit" it!
        /// </summary>
        private void BiteBullet(GameObject bullet)
        {
            bulletsBitten++;

            // Log the bite to the Console
            Debug.Log($"CHOMP! You bit bullet #{bulletsBitten}! ({bullet.name})");

            // Flash the player to show feedback
            TriggerFlash();

            // Tell the spawner a bullet was destroyed
            if (spawner != null)
            {
                spawner.OnBulletDestroyed();
            }

            // Destroy the bullet
            Destroy(bullet);
        }

        /// <summary>
        /// Briefly change the player's color to show they bit a bullet.
        /// </summary>
        private void TriggerFlash()
        {
            if (playerRenderer != null)
            {
                playerRenderer.material.color = flashColor;
                isFlashing = true;
                flashTimer = flashDuration;
            }
        }

        /// <summary>
        /// Display the score on screen using Unity's built-in GUI system.
        /// This is the simplest way to show text without any UI setup.
        /// </summary>
        void OnGUI()
        {
            // Create a style for the score text
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 28;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            // Draw a shadow for readability
            GUIStyle shadow = new GUIStyle(style);
            shadow.normal.textColor = Color.black;

            string scoreText = $"Bullets Bitten: {bulletsBitten}";

            // Shadow
            GUI.Label(new Rect(22, 22, 400, 50), scoreText, shadow);
            // Main text
            GUI.Label(new Rect(20, 20, 400, 50), scoreText, style);

            // Instructions
            GUIStyle instrStyle = new GUIStyle(GUI.skin.label);
            instrStyle.fontSize = 16;
            instrStyle.normal.textColor = new Color(1f, 1f, 1f, 0.7f);

            GUI.Label(new Rect(20, 55, 500, 30), "Move into bullets to 'bite' them! (WASD + Mouse)", instrStyle);
        }
    }
}
