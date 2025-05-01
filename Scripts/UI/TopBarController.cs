using UnityEngine;
using UnityEngine.UI;

public class TopBarController : MonoBehaviour
{
    [Header("UI References")]
    public Text energyText;
    public Text goldText;
    public Text soulsText;
    
    private void Start()
    {
        // Initialize UI
        UpdateUI();
        
        // Subscribe to events
        if (GameManager.Instance != null)
        {
            PlayerData playerData = GameManager.Instance.PlayerData;
            playerData.onEnergyChanged.AddListener(UpdateEnergyText);
            playerData.onGoldChanged.AddListener(UpdateGoldText);
            playerData.onSoulsChanged.AddListener(UpdateSoulsText);
        }
    }
    
    private void UpdateUI()
    {
        if (GameManager.Instance != null)
        {
            PlayerData playerData = GameManager.Instance.PlayerData;
            UpdateEnergyText(playerData.energy);
            UpdateGoldText(playerData.gold);
            UpdateSoulsText(playerData.souls);
        }
    }
    
    private void UpdateEnergyText(int energy)
    {
        if (energyText != null)
        {
            energyText.text = energy.ToString();
        }
    }
    
    private void UpdateGoldText(int gold)
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }
    
    private void UpdateSoulsText(int souls)
    {
        if (soulsText != null)
        {
            soulsText.text = souls.ToString();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            PlayerData playerData = GameManager.Instance.PlayerData;
            playerData.onEnergyChanged.RemoveListener(UpdateEnergyText);
            playerData.onGoldChanged.RemoveListener(UpdateGoldText);
            playerData.onSoulsChanged.RemoveListener(UpdateSoulsText);
        }
    }
} 