using UnityEngine;
using UnityEngine.Events;
public class CharacterBase : MonoBehaviour {
    [Header("Identity")]
    public string unitName = "Character";
    
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float baseDamage = 10f;

    public UnityEvent<float> onHealthChanged;
    public UnityEvent onDeath;

    protected virtual void Start()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;

        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public virtual void TakeDamage(float damage)
    {
        float actualDamage = damage;
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public virtual void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    protected virtual void Die()
    {
        onDeath?.Invoke();
    }
}