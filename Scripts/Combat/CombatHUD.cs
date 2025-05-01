using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatHUD : MonoBehaviour
{
    public TMP_Text nameText;
    public Slider healthSlider;

    public void SetHUD(CharacterBase unit)
    {
        nameText.text = unit.unitName;
        healthSlider.maxValue = unit.maxHealth;
        healthSlider.value = unit.currentHealth;
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }
    
    
}