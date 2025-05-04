using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacter : CharacterBase {
    private PlayerData playerData;
    
    // Events for UI updates
    public UnityEvent<int> onEnergyChanged = new UnityEvent<int>();
    public UnityEvent<int> onGoldChanged = new UnityEvent<int>();
    public UnityEvent<int> onSoulsChanged = new UnityEvent<int>();
    
    public int gold;  // player gold
    public int souls; // player souls

    protected override void Start()
    {
        base.Start();
        
        // Get reference to player data from GameManager
        playerData = GameManager.Instance.PlayerData;
        
        // Set character stats based on player data
        maxHealth = playerData.maxHealth;
        currentHealth = playerData.currentHealth;
        baseDamage = playerData.baseDamage;
        
        // Update UI
        onHealthChanged?.Invoke(currentHealth / maxHealth);
        onEnergyChanged?.Invoke(playerData.energy);
        onGoldChanged?.Invoke(playerData.gold);
        onSoulsChanged?.Invoke(playerData.souls);
        
        // Subscribe to events
        playerData.onHealthChanged.AddListener(UpdateHealth);
        playerData.onEnergyChanged.AddListener(UpdateEnergy);
        playerData.onGoldChanged.AddListener(UpdateGold);
        playerData.onSoulsChanged.AddListener(UpdateSouls);
    }
    
    private void UpdateHealth(int health)
    {
        currentHealth = health;
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }
    
    private void UpdateEnergy(int energy)
    {
        onEnergyChanged?.Invoke(energy);
    }
    
    private void UpdateGold(int gold)
    {
        onGoldChanged?.Invoke(gold);
    }
    
    private void UpdateSouls(int souls)
    {
        onSoulsChanged?.Invoke(souls);
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        // Sync with PlayerData
        playerData.TakeDamage((int)damage);
    }
    
    public override void Heal(float amount)
    {
        base.Heal(amount);
        // Sync with PlayerData
        playerData.Heal((int)amount);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerData != null)
        {
            playerData.onHealthChanged.RemoveListener(UpdateHealth);
            playerData.onEnergyChanged.RemoveListener(UpdateEnergy);
            playerData.onGoldChanged.RemoveListener(UpdateGold);
            playerData.onSoulsChanged.RemoveListener(UpdateSouls);
        }
    }
}