using UnityEngine;
using TMPro;

public class EnergyDisplay : MonoBehaviour
{
    [Header("References")]
    public TMP_Text energyText;
    
    private CombatManager combatManager;
    private int lastEnergyAmount = -1;
    
    private void Start()
    {
        // Find Combat Manager
        combatManager = FindObjectOfType<CombatManager>();
        if (combatManager == null)
        {
            Debug.LogError("Energy Display: Combat Manager not found!");
            return;
        }
        
        // Initialize text
        UpdateEnergyDisplay(combatManager.currentEnergy);
    }
    
    private void Update()
    {
        if (combatManager != null && lastEnergyAmount != combatManager.currentEnergy)
        {
            UpdateEnergyDisplay(combatManager.currentEnergy);
            lastEnergyAmount = combatManager.currentEnergy;
        }
    }
    
    private void UpdateEnergyDisplay(int currentEnergy)
    {
        if (energyText != null)
        {
            energyText.text = currentEnergy.ToString() + " / " + combatManager.maxEnergy.ToString();
        }
    }
} 