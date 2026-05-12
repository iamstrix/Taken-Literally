using UnityEngine;
using UnityEditor;
using TakenLiterally.BiteTheBullet;

namespace TakenLiterally.EditorTools
{
    /// <summary>
    /// Editor tool that builds the entire "Bite the Bullet" scene with one click.
    /// Access it from the Unity menu: Tools > Taken Literally > Build Bite The Bullet Scene
    /// </summary>
    public class BiteTheBulletSetup : EditorWindow
    {
        [MenuItem("Tools/Taken Literally/Build Bite The Bullet Scene")]
        public static void BuildScene()
        {
            // Ensure the "Bullet" tag exists
            EnsureTagExists("Bullet");

            // =============================================
            // 1. CREATE THE FLOOR
            // =============================================
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(30f, 0.5f, 30f);

            // Give the floor a dark color
            Renderer floorRenderer = floor.GetComponent<Renderer>();
            if (floorRenderer != null)
            {
                floorRenderer.material.color = new Color(0.2f, 0.25f, 0.3f);
            }

            // =============================================
            // 2. CREATE BOUNDARY WALLS
            // =============================================
            CreateWall("Wall_North", new Vector3(0f, 2.5f, 15f), new Vector3(30f, 5f, 0.5f));
            CreateWall("Wall_South", new Vector3(0f, 2.5f, -15f), new Vector3(30f, 5f, 0.5f));
            CreateWall("Wall_East",  new Vector3(15f, 2.5f, 0f), new Vector3(0.5f, 5f, 30f));
            CreateWall("Wall_West",  new Vector3(-15f, 2.5f, 0f), new Vector3(0.5f, 5f, 30f));

            // =============================================
            // 3. CREATE THE PLAYER
            // =============================================
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1.5f, 0f);
            player.transform.localScale = new Vector3(1f, 1f, 1f);

            // Give the player a distinct blue color
            Renderer playerRenderer = player.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                playerRenderer.material.color = new Color(0.2f, 0.5f, 1f);
            }

            // Add player scripts
            player.AddComponent<BiteTheBulletPlayer>();
            player.AddComponent<BulletBiter>();

            // =============================================
            // 4. CREATE THE BULLET SPAWNER
            // =============================================
            GameObject spawner = new GameObject("BulletSpawner");
            spawner.transform.position = Vector3.zero;
            spawner.AddComponent<BulletSpawner>();

            // =============================================
            // 5. ADD LIGHTING
            // =============================================
            // Check if a directional light already exists
            Light existingLight = Object.FindAnyObjectByType<Light>();
            if (existingLight == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                light.color = new Color(1f, 0.95f, 0.85f);
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // =============================================
            // 6. DELETE THE DEFAULT MAIN CAMERA
            // =============================================
            // The player script creates its own camera, so remove any existing one
            Camera existingCam = Object.FindAnyObjectByType<Camera>();
            if (existingCam != null)
            {
                Object.DestroyImmediate(existingCam.gameObject);
            }

            // =============================================
            // DONE!
            // =============================================
            Selection.activeGameObject = player;
            Debug.Log("=== BITE THE BULLET SCENE BUILT! ===");
            Debug.Log("Press Play to start. Use WASD to move, Mouse to look, Space to jump.");
            Debug.Log("Walk into the golden bullets to 'bite' them!");

            EditorUtility.DisplayDialog(
                "Bite The Bullet",
                "Scene built successfully!\n\n" +
                "Press PLAY to start.\n" +
                "Use WASD to move, Mouse to look.\n" +
                "Walk into the golden bullets to 'bite' them!",
                "Let's Go!"
            );
        }

        /// <summary>
        /// Creates a wall from a cube primitive.
        /// </summary>
        private static void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;

            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.35f, 0.35f, 0.4f, 0.5f);
            }
        }

        /// <summary>
        /// Makes sure the specified tag exists in the project.
        /// Unity requires tags to be registered before you can use them.
        /// </summary>
        private static void EnsureTagExists(string tagName)
        {
            // Open the TagManager asset
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/TagManager.asset")
            );
            SerializedProperty tags = tagManager.FindProperty("tags");

            // Check if the tag already exists
            for (int i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tagName)
                    return; // Already exists
            }

            // Add the new tag
            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tagName;
            tagManager.ApplyModifiedProperties();

            Debug.Log($"Created new tag: '{tagName}'");
        }
    }
}
