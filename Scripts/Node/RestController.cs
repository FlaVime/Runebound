using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestController : MonoBehaviour
{
    [SerializeField] private Button restButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private int healAmount = 20; // Amount of HP to restore
    
    private PlayerData playerData;
    
    private void Start()
    {
        // Get player data from GameManager
        playerData = GameManager.Instance.PlayerData;
        
        // Set up button listeners
        restButton.onClick.AddListener(RestPlayer);
        returnButton.onClick.AddListener(ReturnToMap);
        
        // Check if player is already at full health
        if (playerData.currentHealth >= playerData.maxHealth)
        {
            restButton.interactable = false;
            if (statusText != null)
            {
                statusText.text = "You are already fully rested.";
            }
        }
        
        // Update UI
        UpdateHealthDisplay();
    }
    
    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {playerData.currentHealth}/{playerData.maxHealth}";
        }
    }
    
    private void RestPlayer()
    {
        // Record health before healing
        int previousHealth = playerData.currentHealth;
        
        // Heal the player using the PlayerData Heal method
        playerData.Heal(healAmount);
        
        // Calculate the actual amount healed
        int healedAmount = playerData.currentHealth - previousHealth;
        
        // Update status text
        if (statusText != null)
        {
            statusText.text = $"You rested and recovered {healedAmount} health.";
        }
        
        // Disable rest button after use
        restButton.interactable = false;
        
        // Update UI
        UpdateHealthDisplay();
        
        // Save game state
        GameManager.Instance.SaveGame();
    }
    
    private void ReturnToMap()
    {
        // Return to map
        GameManager.Instance.ChangeState(GameState.Map);
    }
} 