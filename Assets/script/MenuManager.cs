using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject menuUI;
    private bool isMenuActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    public void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        menuUI.SetActive(isMenuActive);
        Time.timeScale = isMenuActive ? 0 : 1;
    }

    public void ResumeGame()
    {
        ToggleMenu();
    }

    public void RestartGame()
    {
        ToggleMenu();              // 메뉴 닫기 및 시간 재개
        GameManager.Instance.Retry();  // 게임 재시작 함수 호출
    }

    public void QuitGame()
    {
        ToggleMenu();                  // 메뉴 닫기 및 시간 재개
        GameManager.Instance.QuitToMenu(); // 메뉴로 돌아가는 함수 호출
    }
}
