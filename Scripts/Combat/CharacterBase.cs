using UnityEngine;
using UnityEngine.Events;

public class CharacterBase : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 10;
    public int currentHP;

    protected virtual void Awake() {
        currentHP = maxHP;
        Debug.Log($"{name} spawned with {currentHP}/{maxHP} HP");
    }

    public virtual void TakeDamage(int amount) {
        currentHP = Mathf.Max(currentHP - amount, 0);
        Debug.Log($"{name} takes {amount} damage, now {currentHP}/{maxHP} HP");
        if (currentHP == 0) Die();
    }

    protected virtual void Die() {
        Debug.Log($"{name} died");
        Destroy(gameObject);
    }
    }