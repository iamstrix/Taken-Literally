using UnityEngine;

namespace TakenLiterally.BiteTheBullet
{
    /// <summary>
    /// Spawns floating bullets around a center point.
    /// Attach this to an empty GameObject in your scene.
    /// It will create bullet objects at random positions around the player.
    /// </summary>
    public class BulletSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("How many bullets to spawn at the start.")]
        [SerializeField] private int initialBulletCount = 10;

        [Tooltip("The radius around the spawner where bullets appear.")]
        [SerializeField] private float spawnRadius = 10f;

        [Tooltip("The height range for spawning bullets.")]
        [SerializeField] private float spawnHeightMin = 0.5f;
        [SerializeField] private float spawnHeightMax = 2.5f;

        [Header("Bullet Appearance")]
        [Tooltip("The size of each bullet.")]
        [SerializeField] private float bulletSize = 0.4f;

        [Tooltip("The color of the bullets.")]
        [SerializeField] private Color bulletColor = new Color(1f, 0.85f, 0.2f); // Gold

        [Header("Continuous Spawning")]
        [Tooltip("Spawn a new bullet every X seconds. Set to 0 to disable.")]
        [SerializeField] private float respawnInterval = 5f;

        [Tooltip("Maximum bullets allowed in the scene at once.")]
        [SerializeField] private int maxBullets = 15;

        // Track the current bullet count
        private int currentBulletCount = 0;
        private float respawnTimer;

        void Start()
        {
            // Spawn the initial batch of bullets
            for (int i = 0; i < initialBulletCount; i++)
            {
                SpawnBullet();
            }
        }

        void Update()
        {
            // Continuously spawn new bullets if below the max
            if (respawnInterval > 0 && currentBulletCount < maxBullets)
            {
                respawnTimer -= Time.deltaTime;
                if (respawnTimer <= 0f)
                {
                    SpawnBullet();
                    respawnTimer = respawnInterval;
                }
            }
        }

        /// <summary>
        /// Creates a single bullet at a random position around the spawner.
        /// The bullet is built entirely from Unity primitives—no external assets needed.
        /// </summary>
        private void SpawnBullet()
        {
            // Pick a random position within the spawn radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            float randomHeight = Random.Range(spawnHeightMin, spawnHeightMax);
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, randomHeight, randomCircle.y);

            // Create the bullet body (a capsule rotated sideways to look like a bullet)
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bullet.name = "Bullet_" + currentBulletCount;
            bullet.tag = "Bullet";
            bullet.transform.position = spawnPos;
            bullet.transform.localScale = new Vector3(bulletSize, bulletSize * 1.5f, bulletSize);

            // Rotate it sideways so it looks like a bullet, not a person
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            // Give it the gold color
            Renderer renderer = bullet.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = bulletColor;

                // Add a slight metallic sheen
                renderer.material.SetFloat("_Metallic", 0.7f);
                renderer.material.SetFloat("_Smoothness", 0.8f);
            }

            // Add the floating behavior script
            bullet.AddComponent<FloatingBullet>();

            // Track it
            currentBulletCount++;
        }

        /// <summary>
        /// Call this when a bullet is destroyed (e.g., when the player bites it).
        /// This allows the spawner to replace it with a new one.
        /// </summary>
        public void OnBulletDestroyed()
        {
            currentBulletCount--;
        }
    }
}
