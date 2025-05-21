using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    public static GameManager Instance { get; private set; }
    public static bool isRetrying = false;
    public static bool isReturningToMenu = false;

    public GameObject eggPrefab;
    public GameObject nestPrefab;
    public Transform eggSpawnPoint;
    public GameObject gameOverUI;
    public GameObject startMenuUI;
    public Text finalScoreText;

    // 50점 이상 벽 생성
    public GameObject leftBox;
    public GameObject rightBox;
    public GameObject topBox;
    public GameObject circulBox;

    // 클린샷
    private int cleanStreak = 0; 
    private const int maxStreak = 5;

    // 음향
    public AudioClip bounceClip;
    public AudioClip launchClip;
    public AudioClip scoreClip;
    public AudioClip gameOverClip;

    public int score = 0;

    private GameObject currentNest;
    private GameObject currentEgg;
    private GameObject currentWall;

    //음향
    public AudioSource audioSource;

    private bool gameStarted = false;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (isRetrying)
        {
            isRetrying = false;
            Time.timeScale = 1;
            gameStarted = true;
            score = 0;
            scoreText.text = "Score: 0";
            startMenuUI.SetActive(false);
            gameOverUI.SetActive(false);
            SpawnNest();
            SpawnEgg();
        }
        else
        {
            isReturningToMenu = false;
            Time.timeScale = 0;
            startMenuUI.SetActive(true);
            gameOverUI.SetActive(false);
        }
    }


    public void StartGame()
    {
        startMenuUI.SetActive(false);
        Time.timeScale = 1; // 게임 시작
        gameStarted = true;
        score = 0;
        scoreText.text = "Score: 0";
        SpawnNest();
        SpawnEgg();
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중단
#endif
    }
    public void Retry()
    {
        isRetrying = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        isReturningToMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnEggScored(int bounceCount, bool touchedNest)
    {
        // 클린샷 처리
        if (!touchedNest)
            cleanStreak = Mathf.Min(cleanStreak + 1, maxStreak);
        else
            cleanStreak = 0;

        int baseScore = cleanStreak > 0 ? cleanStreak : 1;
        int multiplier = (int)Mathf.Pow(2, bounceCount);
        int total = baseScore * multiplier;

        score += total;
        UpdateScoreUI();

        Destroy(currentNest);
        Destroy(currentEgg);
        StartCoroutine(DelayedSpawn());
    }

    public void AddBounceScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    public void OnGameOver()
    {
        audioSource.PlayOneShot(gameOverClip);
        gameOverUI.SetActive(true);
        finalScoreText.text = $"Final Score: {score}";
        Time.timeScale = 0;
    }

    public void SpawnEgg()
    {
        if (currentEgg != null)
            Destroy(currentEgg);

        float x = Random.Range(-2f, 2f); // 좌우만 랜덤
        float y = eggSpawnPoint.position.y; // 기존 Y 위치 유지
        Vector3 pos = new Vector3(x, y, 0);
        currentEgg = Instantiate(eggPrefab, pos, Quaternion.identity);
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.4f);
        SpawnNest();
        SpawnEgg();
    }

    public void SpawnNest()
    {
        if (currentNest != null)
            Destroy(currentNest);

        if (currentWall != null)
            Destroy(currentWall);

        float x = Random.Range(-4, 5) * 0.5f;
        float y = Random.Range(-1f, 2.5f);
        Vector3 nestPos = new Vector3(x, y, 0);

        if (score > 50)
        {
            int type = Random.Range(0, 5); // 0~4까지 (5개 경우)
            switch (type)
            {
                case 0: // 기본 둥지
                    currentNest = Instantiate(nestPrefab, nestPos, Quaternion.identity);
                    break;
                case 1: // 좌벽 둥지
                    currentNest = Instantiate(nestPrefab, nestPos, Quaternion.identity);
                    currentWall = Instantiate(leftBox, nestPos + Vector3.left * 1.2f, Quaternion.identity);
                    break;
                case 2: // 우벽 둥지
                    currentNest = Instantiate(nestPrefab, nestPos, Quaternion.identity);
                    currentWall = Instantiate(rightBox, nestPos + Vector3.right * 1.2f, Quaternion.identity);
                    break;
                case 3: // 상벽 둥지
                    currentNest = Instantiate(nestPrefab, nestPos, Quaternion.identity);
                    currentWall = Instantiate(topBox, nestPos + Vector3.up * 1.2f, Quaternion.identity);
                    break;
                case 4: // 랜덤 원형 벽 (둥지와 거리 유지)
                    currentNest = Instantiate(nestPrefab, nestPos, Quaternion.identity);

                    Vector3 wallPos;
                    int maxAttempts = 10;
                    float minDistance = 1.5f; // 겹치지 않도록 최소 거리 설정

                    do
                    {
                        float wx = Random.Range(-2f, 2f);
                        float wy = Random.Range(-1f, 3f);
                        wallPos = new Vector3(wx, wy, 0);
                        maxAttempts--;
                    }
                    while (Vector3.Distance(wallPos, nestPos) < minDistance && maxAttempts > 0);

                    currentWall = Instantiate(circulBox, wallPos, Quaternion.identity);
                    break;
            }
        }
        else
        {
            currentNest = Instantiate(nestPrefab, nestPos, Quaternion.identity);
        }
    }

}
