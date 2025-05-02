using UnityEngine;
using TMPro;

public class TopBarResourceDisplay : MonoBehaviour
{
    [Header("Resource Text References")]
    public TMP_Text goldText;
    public TMP_Text soulsText;
    public TMP_Text energyText;
    public TMP_Text healthText;
    
    [Header("Format")]
    public bool showIcons = true;
    public string goldPrefix = "";
    public string soulsPrefix = "";
    public string energyPrefix = "";
    public string healthPrefix = "";
    
    private PlayerData playerData;
    
    private void Start()
    {
        // Get a reference to the player data
        if (GameManager.Instance != null)
        {
            playerData = GameManager.Instance.PlayerData;
            
            // Subscribe to resource change events
            if (playerData != null)
            {
                playerData.onGoldChanged.AddListener(UpdateGoldText);
                playerData.onSoulsChanged.AddListener(UpdateSoulsText);
                playerData.onEnergyChanged.AddListener(UpdateEnergyText);
                playerData.onHealthChanged.AddListener(UpdateHealthText);
                
                // Initial update
                UpdateAllResources();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events when this object is destroyed
        if (playerData != null)
        {
            playerData.onGoldChanged.RemoveListener(UpdateGoldText);
            playerData.onSoulsChanged.RemoveListener(UpdateSoulsText);
            playerData.onEnergyChanged.RemoveListener(UpdateEnergyText);
            playerData.onHealthChanged.RemoveListener(UpdateHealthText);
        }
    }
    
    public void UpdateAllResources()
    {
        if (playerData != null)
        {
            UpdateGoldText(playerData.gold);
            UpdateSoulsText(playerData.souls);
            UpdateEnergyText(playerData.energy);
            UpdateHealthText(playerData.currentHealth);
        }
    }
    
    private void UpdateGoldText(int gold)
    {
        if (goldText != null)
        {
            goldText.text = goldPrefix + gold.ToString();
        }
    }
    
    private void UpdateSoulsText(int souls)
    {
        if (soulsText != null)
        {
            soulsText.text = soulsPrefix + souls.ToString();
        }
    }
    
    private void UpdateEnergyText(int energy)
    {
        if (energyText != null)
        {
            energyText.text = energyPrefix + energy.ToString() + "/" + playerData.maxEnergy.ToString();
        }
    }
    
    private void UpdateHealthText(int health)
    {
        if (healthText != null)
        {
            healthText.text = healthPrefix + health.ToString() + "/" + playerData.maxHealth.ToString();
        }
    }
} 