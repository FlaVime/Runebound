using UnityEngine;
using UnityEngine.Events;

public class Enemy : CharacterBase
{
    [Header("Enemy Data")]
    public EnemyData enemyData;
    
    [Header("AI Settings")]
    [Range(0f, 1f)]
    public float skipTurnChance = 0.2f;
    
    [Header("Rewards")]
    public int goldReward = 10;
    public int soulsReward = 5;
    
    public UnityEvent<float> onAttackPlayer;
    public UnityEvent onEnemyDefeated;

    private void Awake()
    {
        if (enemyData != null)
        {
            // Apply data from ScriptableObject
            unitName = enemyData.enemyName;
            maxHealth = enemyData.maxHealth;
            baseDamage = enemyData.baseDamage;
            skipTurnChance = enemyData.skipTurnChance;
            goldReward = enemyData.goldReward;
            soulsReward = enemyData.soulsReward;
        }
    }

    protected override void Start()
    {
        base.Start();
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
        // Attack logic is handled by CombatManager
        onAttackPlayer?.Invoke(baseDamage);
    }

    protected override void Die()
    {
        base.Die();
        
        // Give rewards to player
        if (GameManager.Instance != null)
            GameManager.Instance.GiveRewards(goldReward, soulsReward);
        
        onEnemyDefeated?.Invoke();
        Destroy(gameObject);
    }
} 