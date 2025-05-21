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
    protected float defenseMultiplier = 1f;

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
        float defenseFactor = 1f / (1f + playerData.baseDefense);
        float actualDamage = damage * defenseFactor * defenseMultiplier;
        playerData.TakeDamage(Mathf.RoundToInt(actualDamage));
    }
    
    public override void Heal(float amount)
    {
        base.Heal(amount);
        playerData.Heal((int)amount);
    }

    public virtual void SetDefenseMultiplier(float multiplier)
    {
        defenseMultiplier = multiplier;
    }

    public virtual void ResetDefenseMultiplier()
    {
        defenseMultiplier = 1f;
    }
    
    private void OnDestroy()
    {
        if (playerData != null)
        {
            playerData.onHealthChanged.RemoveListener(UpdateHealth);
            playerData.onEnergyChanged.RemoveListener(UpdateEnergy);
            playerData.onGoldChanged.RemoveListener(UpdateGold);
            playerData.onSoulsChanged.RemoveListener(UpdateSouls);
        }
    }
}