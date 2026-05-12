using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Collider))]
public class PetCatcher : MonoBehaviour
{
    [Header("Goal")]
    public int catGoal = 5;
    public int dogGoal = 5;

    [Header("UI")]
    public TextMeshProUGUI catCountText;
    public TextMeshProUGUI dogCountText;
    public GameObject winPanel;

    [Header("Catch Effects")]
    public GameObject catCatchEffect;
    public GameObject dogCatchEffect;

    [Header("Scene Transition")]
    public string nextScene = "1.hub";
    public float winDelay = 3f;

    private int catsCaught;
    private int dogsCaught;
    private bool gameWon;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (winPanel != null) winPanel.SetActive(false);
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameWon) return;

        FallingPet pet = other.GetComponent<FallingPet>();
        if (pet == null) return;

        if (pet.type == PetType.Cat)
        {
            catsCaught = Mathf.Min(catsCaught + 1, catGoal);
            SpawnEffect(catCatchEffect, other.transform.position);
        }
        else
        {
            dogsCaught = Mathf.Min(dogsCaught + 1, dogGoal);
            SpawnEffect(dogCatchEffect, other.transform.position);
        }

        Destroy(other.gameObject);
        UpdateUI();
        CheckWin();
    }

    void SpawnEffect(GameObject effectPrefab, Vector3 position)
    {
        if (effectPrefab == null) return;
        GameObject fx = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(fx, 2f);
    }

    void UpdateUI()
    {
        if (catCountText != null) catCountText.text = $"Cats: {catsCaught} / {catGoal}";
        if (dogCountText != null) dogCountText.text = $"Dogs: {dogsCaught} / {dogGoal}";
    }

    void CheckWin()
    {
        if (catsCaught >= catGoal && dogsCaught >= dogGoal)
        {
            gameWon = true;
            if (winPanel != null) winPanel.SetActive(true);
            Invoke(nameof(LoadNext), winDelay);
        }
    }

    void LoadNext() => SceneManager.LoadScene(nextScene);
}
