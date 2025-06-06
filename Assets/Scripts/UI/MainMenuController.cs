using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    
    public Button continueButton;

    private void Start()
    {
        // Check if there's a saved game to enable/disable continue button
        if (continueButton != null)
        {
            bool hasSave = PlayerPrefs.HasKey("SavedGame");
            continueButton.interactable = hasSave;
        }
    }

    public void NewGame() 
    {
        GameManager.Instance.StartNewGame();
    }
    
    public void ContinueGame() 
    {
        GameManager.Instance.ContinueGame();
    }

    public void Options() 
    {
        if (optionsMenu != null) optionsMenu.SetActive(true);
        if (mainMenu != null) mainMenu.SetActive(false);
    }

    public void BackToMain() 
    {
        if (optionsMenu != null) optionsMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    public void ExitGame() 
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
