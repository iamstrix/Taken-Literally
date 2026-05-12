using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // Ensure you have TextMeshPro installed

public class ElephantHunter : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactDistance = 3.0f;
    public string elephantTag = "Elephant";
    public LayerMask interactLayer; // Set this to 'Default' in the Inspector

    [Header("UI Elements")]
    public GameObject interactPrompt; // The "Press E to Interact" UI object

    [Header("Effects")]
    public ParticleSystem elephantParticles;

    void Update()
    {
        CheckForElephant();
    }

    void CheckForElephant()
    {
        // Shoot a ray from the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.CompareTag(elephantTag))
            {
                // Show the prompt
                if (interactPrompt != null) interactPrompt.SetActive(true);

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Debug.Log("E pressed while looking at elephant!");
                    FoundElephant(hit.point);
                }
                return;
            }
        }

        // Hide prompt if not looking at elephant or too far
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void FoundElephant(Vector3 hitPoint)
    {
        Debug.Log($"FoundElephant triggered at {hitPoint}");
        
        if (elephantParticles != null)
        {
            elephantParticles.transform.position = hitPoint;
            elephantParticles.Play();
            Debug.Log("Particle system .Play() called.");
        }
        else
        {
            Debug.LogWarning("Elephant Particles reference is MISSING in the inspector!");
        }
    }
}