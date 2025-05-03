using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public int maxEnergy = 3;
    public int energy;
    
    public int gold;
    public int souls;
    
    // Combat stats
    public float baseDamage = 10f;
    public float baseDefense = 1f;
    
    // Store IDs of purchased upgrades
    public List<string> purchasedUpgrades = new List<string>();
    
    // Events
    public UnityEvent<int> onHealthChanged = new UnityEvent<int>();
    public UnityEvent<int> onGoldChanged = new UnityEvent<int>();
    public UnityEvent<int> onSoulsChanged = new UnityEvent<int>();
    public UnityEvent<int> onEnergyChanged = new UnityEvent<int>();
    
    public PlayerData()
    {
        currentHealth = maxHealth;
        energy = maxEnergy;
    }
    
    public void Init()
    {
        currentHealth = maxHealth;
        energy = maxEnergy;
        gold = 100; // Starting gold
        souls = 20; // Starting souls
        purchasedUpgrades.Clear();
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
    
    public bool HasUpgrade(string upgradeId)
    {
        return purchasedUpgrades.Contains(upgradeId);
    }
    
    public void AddUpgrade(string upgradeId)
    {
        if (!purchasedUpgrades.Contains(upgradeId))
        {
            purchasedUpgrades.Add(upgradeId);
        }
    }
} 