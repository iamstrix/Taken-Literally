using UnityEngine;
using UnityEngine.InputSystem;

public class ElephantHunter : MonoBehaviour
{
    [Header("Detection Settings")]
    public float proximityDistance = 5.0f;
    public float interactDistance = 3.0f;
    public string elephantTag = "Elephant";
    public LayerMask interactLayer;

    [Header("UI Elements")]
    public GameObject interactPrompt;

    [Header("Effects")]
    public ParticleSystem elephantParticles;

    void Update()
    {
        CheckForElephant();
    }

    void CheckForElephant()
    {
        if (!IsElephantNearby())
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool isLooking = Physics.Raycast(ray, out hit, interactDistance, interactLayer)
                         && hit.collider.CompareTag(elephantTag);

        if (interactPrompt != null) interactPrompt.SetActive(isLooking);

        if (isLooking && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            FoundElephant(hit.point);
    }

    bool IsElephantNearby()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, proximityDistance);
        foreach (var col in nearby)
        {
            if (col.CompareTag(elephantTag))
                return true;
        }
        return false;
    }

    void FoundElephant(Vector3 hitPoint)
    {
        if (elephantParticles != null)
        {
            elephantParticles.transform.position = hitPoint;
            elephantParticles.Play();
        }
        else
        {
            Debug.LogWarning("Elephant Particles reference is MISSING in the inspector!");
        }
    }
}