using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    private bool isPaused = false;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Map") return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        pauseCanvas?.SetActive(isPaused);
        pauseMenu?.SetActive(isPaused);
        settingsMenu?.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseCanvas?.SetActive(false);
        pauseMenu?.SetActive(false);
        settingsMenu?.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenu?.SetActive(false);
        settingsMenu?.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu?.SetActive(false);
        pauseMenu?.SetActive(true);
    }

    public void ExitToMenu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
