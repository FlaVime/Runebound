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
        slider.interactable = false;
    }

    public void SetCharacter(CharacterBase character)
    {
        // Unsubscribe from the previous character's health change event
        if (this.character != null)
            this.character.onHealthChanged.RemoveListener(UpdateHealth);

        this.character = character;
        
        if (character != null)
        {
            slider.maxValue = character.maxHealth;
            slider.value = character.currentHealth;
            character.onHealthChanged.AddListener(UpdateHealth);
        }
    }

    private void UpdateHealth(float healthPercent)
    {
        if (slider == null || character == null) return;
        
        float healthValue = character.maxHealth * healthPercent;
        slider.value = healthValue;
    }

    public void SetHealth(float currentHealth)
    {
        if (slider == null) return;

        slider.value = currentHealth;
    }

    private void OnDestroy()
    {
        if (character != null)
            character.onHealthChanged.RemoveListener(UpdateHealth);
    }
} 