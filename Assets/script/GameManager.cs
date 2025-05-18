using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool isRetrying = false;
    public static bool isReturningToMenu = false;

    public GameObject eggPrefab;
    public GameObject nestPrefab;
    public Transform eggSpawnPoint;
    public Text scoreText;
    public GameObject gameOverUI;
    public GameObject startMenuUI;
    public Text finalScoreText;

    private GameObject currentNest;
    private GameObject currentEgg;
    private int score = 0;
    private bool gameStarted = false;

    void Awake() => Instance = this;

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

    public void OnEggScored()
    {
        score++;
        scoreText.text = $"Score: {score}";
        Destroy(currentNest);
        Destroy(currentEgg);
        StartCoroutine(DelayedSpawn());
    }

    public void OnGameOver()
    {
        gameOverUI.SetActive(true);
        finalScoreText.text = $"Final Score: {score}";
        Time.timeScale = 0;
    }

    public void SpawnEgg()
    {
        if (currentEgg != null)
            Destroy(currentEgg); 
        Vector3 pos = eggSpawnPoint.position;
        currentEgg = Instantiate(eggPrefab, pos, Quaternion.identity);
    }

    public void SpawnNest()
    {
        if (currentNest != null)
            Destroy(currentNest); // 기존 둥지 제거

        float x = Random.Range(-2f, 2f);
        float y = Random.Range(-1f, 3f);
        currentNest = Instantiate(nestPrefab, new Vector3(x, y, 0), Quaternion.identity);
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.4f);
        SpawnNest();
        SpawnEgg();
    }
}
