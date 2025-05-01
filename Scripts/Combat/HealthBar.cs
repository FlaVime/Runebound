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
    }

    public void SetCharacter(CharacterBase character)
    {
        this.character = character;
        character.onHealthChanged.AddListener(UpdateHealth);
        UpdateHealth(character.currentHealth / character.maxHealth);
    }

    private void UpdateHealth(float healthPercent)
    {
        slider.value = healthPercent;
    }

    private void OnDestroy()
    {
        if (character != null)
        {
            character.onHealthChanged.RemoveListener(UpdateHealth);
        }
    }
} 