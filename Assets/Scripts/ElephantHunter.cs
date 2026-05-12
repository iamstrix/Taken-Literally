using UnityEngine;
using TMPro; // Ensure you have TextMeshPro installed

public class ElephantHunter : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactDistance = 3.0f;
    public string elephantTag = "Elephant";
    public LayerMask interactLayer; // Set this to 'Default' in the Inspector

    [Header("UI Elements")]
    public GameObject interactPrompt; // The "Press E to Interact" UI object

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
                interactPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    FoundElephant();
                }
                return;
            }
        }

        // Hide prompt if not looking at elephant or too far
        interactPrompt.SetActive(false);
    }

    void FoundElephant()
    {
        Debug.Log("Success! You found the elephant in the room.");
        // Add logic here to load the next stage or trigger a win screen
    }
}