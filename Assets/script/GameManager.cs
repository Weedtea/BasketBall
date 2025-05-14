using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject eggPrefab;
    public GameObject nestPrefab;
    public Transform eggSpawnPoint; // 예: (0, -4, 0)에 빈 오브젝트 배치
    public Text scoreText;
    public GameObject gameOverUI;

    private GameObject currentNest;
    private GameObject currentEgg;
    private int score = 0;

    void Awake() => Instance = this;

    void Start()
    {
        SpawnEgg();
        SpawnNest();
    }

    public void OnEggScored()
    {
        score++;
        scoreText.text = $"Score: {score}";
        
        Destroy(currentNest);
        SpawnNest();
        SpawnEgg();
    }

    public void OnGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void SpawnEgg()
    {
        Vector3 pos = new Vector3(0, -3f, 0); // 항상 고정 위치
        currentEgg = Instantiate(eggPrefab, pos, Quaternion.identity);
        StartCoroutine(currentEgg.GetComponent<Egg>().FadeIn());
    }


    public void SpawnNest()
    {
        float x = Random.Range(-2f, 2f);
        float y = Random.Range(-1f, 3f);
        currentNest = Instantiate(nestPrefab, new Vector3(x, y, 0), Quaternion.identity);
        StartCoroutine(currentNest.GetComponent<Nest>().FadeIn());
    }

}
