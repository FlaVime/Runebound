using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Runebound/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Enemy";
    
    [Header("Stats")]
    public float maxHealth = 100f;
    public float baseDamage = 10f;
    
    [Header("AI Settings")]
    [Range(0f, 1f)]
    public float skipTurnChance = 0.2f;
    
    [Header("Rewards")]
    public int goldReward = 10;
    public int soulsReward = 5;
    
    [Header("Special Abilities")]
    public bool hasSpecialAbility;
    public float specialAbilityChance = 0.3f;
    public float specialAbilityDamage = 15f;
} 