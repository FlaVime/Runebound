using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private CharacterBase character;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        
        // Disable slider interactivity
        slider.interactable = false;
    }

    public void SetCharacter(CharacterBase character)
    {
        this.character = character;
        
        // Properly initialize the slider
        if (character != null)
        {
            slider.maxValue = character.maxHealth;
            slider.value = character.currentHealth;
            
            // Subscribe to health change events
            character.onHealthChanged.AddListener(UpdateHealth);
        }
    }

    // Update based on health percentage
    private void UpdateHealth(float healthPercent)
    {
        if (slider != null)
        {
            // Convert percentage to absolute value
            float healthValue = character.maxHealth * healthPercent;
            slider.value = healthValue;
        }
    }

    // Update with absolute health value
    public void SetHealth(float currentHealth)
    {
        if (slider != null)
        {
            slider.value = currentHealth;
        }
    }

    private void OnDestroy()
    {
        if (character != null)
        {
            character.onHealthChanged.RemoveListener(UpdateHealth);
        }
    }
} 