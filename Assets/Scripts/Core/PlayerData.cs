using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int maxHealth = 100;
    public ProjectedInt currentHealth;
    
    public int maxEnergy = 3;
    public ProjectedInt energy;
    
    public ProjectedInt gold;
    public ProjectedInt souls;
    
    public ProjectedFloat baseDamage;
    public ProjectedFloat baseDefense;
    
    public List<string> purchasedUpgrades = new List<string>();
    
    public UnityEvent<int> onHealthChanged = new UnityEvent<int>();
    public UnityEvent<int> onGoldChanged = new UnityEvent<int>();
    public UnityEvent<int> onSoulsChanged = new UnityEvent<int>();
    public UnityEvent<int> onEnergyChanged = new UnityEvent<int>();
    
    public void Init()
    {
        gold.Set(100); // Starting gold
        souls.Set(200); // Starting souls
        currentHealth.Set(maxHealth);
        energy.Set(maxEnergy);
        baseDamage.Set(15f); // Starting base damage
        baseDefense.Set(1f); // Starting base defense
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

    public void ApplyReward(RewardSystem.Reward reward)
    {
        switch (reward.type)
        {
            case RewardSystem.RewardType.Gold: AddGold(reward.value); break;
            case RewardSystem.RewardType.Souls: AddSouls(reward.value); break;
            case RewardSystem.RewardType.Health: Heal(reward.value); break;
        }
    }

    public void HandleDefeat()
    {
        int currentGold = gold;
        int currentSouls = souls;

        int reducedGold = Mathf.RoundToInt(currentGold * 0.6f);
        int reducedSouls = Mathf.RoundToInt(currentSouls * 0.6f);

        GameLogger.Log($"Defeat: Gold {currentGold} -> {reducedGold}, Souls {currentSouls} -> {reducedSouls}");

        gold.Set(reducedGold);
        souls.Set(reducedSouls);

        currentHealth.Set(maxHealth);
        energy.Set(maxEnergy);

        onGoldChanged?.Invoke(gold);
        onSoulsChanged?.Invoke(souls);
    }
}