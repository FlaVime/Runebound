using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu; // Reference to the main menu GameObject
    public GameObject optionsMenu; // Reference to the options menu GameObject

    public void NewGame() {
        // Переход на сцену Map
        GameManager.Instance.ChangeState(GameState.Map);
    }

    public void Options() {
        if (optionsMenu != null) optionsMenu.SetActive(true);
        if (mainMenu != null) mainMenu.SetActive(false);
    }

    public void BackToMain() {
        if (optionsMenu != null) optionsMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
    }


    public void ExitGame() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // для выхода в редакторе
#endif
    }
}
