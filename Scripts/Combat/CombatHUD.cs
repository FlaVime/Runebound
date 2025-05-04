using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatHUD : MonoBehaviour
{
    public TMP_Text nameText;
    public Slider healthSlider;
    
    private HealthBar healthBar;

    private void Awake()
    {
        // Make sure the slider is not interactive
        if (healthSlider != null)
        {
            healthSlider.interactable = false;
        }
        
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public void SetHUD(CharacterBase unit)
    {
        if (unit != null)
        {
            if (string.IsNullOrEmpty(unit.unitName))
            {
                nameText.text = "Unknown";
            }
            else
            {
                nameText.text = unit.unitName;
            }
            
            // Set up the health bar
            if (healthBar != null)
            {
                healthBar.SetCharacter(unit);
            }
            else if (healthSlider != null)
            {
                healthSlider.maxValue = unit.maxHealth;
                healthSlider.value = unit.currentHealth;
            }
        }
    }

    public void SetHealth(float health)
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
        else if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }
}