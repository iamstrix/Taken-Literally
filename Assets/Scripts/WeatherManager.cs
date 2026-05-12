using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] catPrefabs;
    public GameObject[] dogPrefabs;
    public float spawnRate = 1.5f;
    public float spawnHeight = 20f;
    public float spawnRadius = 12f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnPet();
            timer = 0f;
        }
    }

    void SpawnPet()
    {
        GameObject prefab = PickPrefab();
        if (prefab == null) return;

        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(circle.x, spawnHeight, circle.y);
        Quaternion rot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        GameObject pet = Instantiate(prefab, spawnPos, rot);

        if (pet.GetComponent<Rigidbody>() == null)
            pet.AddComponent<Rigidbody>();

        Destroy(pet, 10f);
    }

    GameObject PickPrefab()
    {
        bool hasCats = catPrefabs.Length > 0;
        bool hasDogs = dogPrefabs.Length > 0;

        if (hasCats && hasDogs)
            return Random.value > 0.5f
                ? catPrefabs[Random.Range(0, catPrefabs.Length)]
                : dogPrefabs[Random.Range(0, dogPrefabs.Length)];

        if (hasCats) return catPrefabs[Random.Range(0, catPrefabs.Length)];
        if (hasDogs) return dogPrefabs[Random.Range(0, dogPrefabs.Length)];

        return null;
    }
}
