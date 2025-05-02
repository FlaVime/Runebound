using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu; // Reference to the main menu GameObject
    public GameObject optionsMenu; // Reference to the options menu GameObject
    
    public Button continueButton; // Reference to continue button (if it exists)

    private void Start()
    {
        // Check if there's a saved game to enable/disable continue button
        if (continueButton != null)
        {
            bool hasSave = PlayerPrefs.HasKey("SavedGame");
            continueButton.interactable = hasSave;
        }
    }

    public void NewGame() {
        // Reset game and go to map using the proper new game method
        GameManager.Instance.StartNewGame();
    }
    
    public void ContinueGame() {
        // Load saved game and continue
        GameManager.Instance.LoadGame();
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
