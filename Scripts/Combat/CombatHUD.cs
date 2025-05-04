using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatHUD : MonoBehaviour
{
    public TMP_Text nameText;
    public Slider healthSlider;
    
    // Reference to the HealthBar component if used
    private HealthBar healthBar;

    private void Awake()
    {
        // Make sure the slider is not interactive
        if (healthSlider != null)
        {
            healthSlider.interactable = false;
        }
        
        // Try to find the HealthBar component
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public void SetHUD(CharacterBase unit)
    {
        if (unit != null)
        {
            // Make sure the name is not empty
            if (string.IsNullOrEmpty(unit.unitName))
            {
                Debug.LogWarning("Unit name is empty, using default name");
                nameText.text = "Unknown";
            }
            else
            {
                nameText.text = unit.unitName;
                Debug.Log($"Setting HUD name to: {unit.unitName}");
            }
            
            // Set up the health bar
            if (healthBar != null)
            {
                // If using the HealthBar component
                healthBar.SetCharacter(unit);
            }
            else if (healthSlider != null)
            {
                // If using just a Slider
                healthSlider.maxValue = unit.maxHealth;
                healthSlider.value = unit.currentHealth;
            }
        }
        else
        {
            Debug.LogError("Trying to set HUD for null unit!");
        }
    }

    public void SetHealth(float health)
    {
        if (healthBar != null)
        {
            // If using the HealthBar component
            healthBar.SetHealth(health);
        }
        else if (healthSlider != null)
        {
            // If using just a Slider
            healthSlider.value = health;
        }
    }
}