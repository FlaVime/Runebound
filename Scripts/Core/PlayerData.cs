using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerData
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public int maxEnergy = 3;
    public int energy;
    
    public int gold;
    public int souls;
    
    // Equipment and items
    public int[] equippedItems = new int[3]; // Array of item IDs
    public int[] inventory = new int[10]; // Array of item IDs
    
    // Combat stats
    public float baseDamage = 10f;
    public float baseDefense = 1f;
    
    // Progression
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    
    // Events
    public UnityEvent<int> onHealthChanged = new UnityEvent<int>();
    public UnityEvent<int> onGoldChanged = new UnityEvent<int>();
    public UnityEvent<int> onSoulsChanged = new UnityEvent<int>();
    public UnityEvent<int> onEnergyChanged = new UnityEvent<int>();
    public UnityEvent<int> onLevelUp = new UnityEvent<int>();
    
    public PlayerData()
    {
        currentHealth = maxHealth;
        energy = maxEnergy;
    }
    
    public void Init()
    {
        currentHealth = maxHealth;
        energy = maxEnergy;
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
        onGoldChanged?.Invoke(gold);
    }
    
    public void AddSouls(int amount)
    {
        souls += amount;
        onSoulsChanged?.Invoke(souls);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void AddEnergy(int amount)
    {
        energy = Mathf.Min(maxEnergy, energy + amount);
        onEnergyChanged?.Invoke(energy);
    }
    
    public void SpendEnergy(int amount)
    {
        energy = Mathf.Max(0, energy - amount);
        onEnergyChanged?.Invoke(energy);
    }
    
    public void AddExperience(int amount)
    {
        experience += amount;
        
        // Check for level up
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = (int)(experienceToNextLevel * 1.5f);
        
        // Increase stats
        maxHealth += 10;
        currentHealth = maxHealth;
        baseDamage += 2f;
        
        onLevelUp?.Invoke(level);
    }
} 