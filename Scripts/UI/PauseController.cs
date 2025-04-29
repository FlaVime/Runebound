using UnityEngine;
using UnityEngine.SceneManagement;

// фиксить проблемы (пропадают ноды становяться невидымыми если on off, можно на них нажимать)
public class PauseController : MonoBehaviour {
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject blocker; // объект с Image + Raycast Target для блокировки кликов

    private bool isPaused = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (settingsMenu.activeSelf) {
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause() {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        blocker.SetActive(isPaused); // включаем блокировщик
    }

    public void Resume() {
        isPaused = false;
        pauseMenu.SetActive(false);
        blocker.SetActive(false);
    }

    public void OpenSettings() {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ExitToMenu() {
        isPaused = false;
        blocker.SetActive(false);
        SceneManager.LoadScene("MainMenu"); // или через GameManager
    }
}
