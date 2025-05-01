using UnityEngine;
using UnityEngine.Events;

public class Enemy : CharacterBase
{
    [Header("AI Settings")]
    [Range(0f, 1f)]
    public float skipTurnChance = 0.2f;
    
    [Header("Rewards")]
    public int goldReward = 10;
    public int soulsReward = 5;
    public int experienceReward = 20;
    
    public UnityEvent<float> onAttackPlayer;
    public UnityEvent onEnemyDefeated;

    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        
        // Find player controller reference
        player = FindObjectOfType<PlayerController>();
    }

    public void TakeTurn()
    {
        if (Random.value > skipTurnChance)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (player != null)
        {
            // Direct attack on player
            player.TakeDamage(baseDamage);
        }
        
        // Trigger event for UI or other listeners
        onAttackPlayer?.Invoke(baseDamage);
    }

    protected override void Die()
    {
        base.Die();
        
        // Give rewards to player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GiveRewards(goldReward, soulsReward, experienceReward);
        }
        
        // Trigger event
        onEnemyDefeated?.Invoke();
        
        // Deactivate object
        gameObject.SetActive(false);
    }
} 