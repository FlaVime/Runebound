using UnityEngine;
using UnityEngine.UI;

public class VictoryPanelController : MonoBehaviour
{
    [SerializeField] private Button againButton;
    [SerializeField] private Button exitButton;
    
    private void OnEnable()
    {
        // Setup when panel is enabled
        SetupButtons();
    }
    
    private void SetupButtons()
    {
        // Настройка кнопок
        if (againButton != null)
            againButton.onClick.AddListener(OnAgainButtonClick);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClick);
    }
    
    private void OnAgainButtonClick()
    {
        // Запуск новой игры с сохранением ресурсов
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
    }
    
    private void OnExitButtonClick()
    {
        // Возврат в главное меню
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
    
    private void OnDisable()
    {
        CleanupListeners();
    }
    
    private void OnDestroy()
    {
        CleanupListeners();
    }
    
    private void CleanupListeners()
    {
        if (againButton != null)
            againButton.onClick.RemoveListener(OnAgainButtonClick);
        
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClick);
    }
} 