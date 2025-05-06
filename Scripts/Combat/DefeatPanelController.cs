using UnityEngine;
using UnityEngine.UI;

public class DefeatPanelController : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    
    private void Start()
    {
        // Set up button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClick);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClick);
    }
    
    private void OnRestartButtonClick()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartAfterDefeat();
    }
    
    private void OnExitButtonClick()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameState.MainMenu);
    }
    
    private void OnDestroy()
    {
        // Clean up listeners
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartButtonClick);
        
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClick);
    }
} 