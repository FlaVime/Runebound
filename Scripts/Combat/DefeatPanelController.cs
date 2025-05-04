using UnityEngine;
using UnityEngine.UI;

public class DefeatPanelController : MonoBehaviour
{
    [SerializeField] private Button againButton;
    [SerializeField] private Button exitButton;
    
    private void Awake()
    {
        // Check if buttons are assigned
        if (againButton == null)
        {
            Debug.LogError("DefeatPanelController: Again button is not assigned!");
        }
        
        if (exitButton == null)
        {
            Debug.LogError("DefeatPanelController: Exit button is not assigned!");
        }
    }
    
    private void Start()
    {
        // Set up button listeners
        if (againButton != null)
        {
            againButton.onClick.AddListener(OnAgainButtonClick);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClick);
        }
    }
    
    private void OnAgainButtonClick()
    {
        // Start a completely new game with a new map
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
    }
    
    private void OnExitButtonClick()
    {
        // Return to main menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up listeners
        if (againButton != null)
        {
            againButton.onClick.RemoveListener(OnAgainButtonClick);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClick);
        }
    }
} 