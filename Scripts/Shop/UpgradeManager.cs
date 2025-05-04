using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public Sprite healthIcon;
    public Sprite damageIcon;
    public Sprite defenseIcon;

    public ShopItem[] upgradeSlots;

    // Upgrade IDs
    private const string HEALTH_UPGRADE = "max_health";
    private const string DAMAGE_UPGRADE = "damage_boost";  
    private const string DEFENSE_UPGRADE = "defense_boost";

    private void Start()
    {
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        var playerData = GameManager.Instance.PlayerData;

        // Health upgrade
        upgradeSlots[0].Init("Max Health +20", "Permanently increase max HP", healthIcon, 50, ShopItem.CurrencyType.Souls, () =>
        {
            playerData.maxHealth += 20;
            playerData.Heal(20); // Also heal to show the effect
            playerData.AddUpgrade(HEALTH_UPGRADE);
            DisableUpgradeButton(upgradeSlots[0]);
        });

        // Damage upgrade
        upgradeSlots[1].Init("Base Damage +2", "Increase your attack power", damageIcon, 75, ShopItem.CurrencyType.Souls, () =>
        {
            playerData.baseDamage += 2;
            playerData.AddUpgrade(DAMAGE_UPGRADE);
            DisableUpgradeButton(upgradeSlots[1]);
        });

        // Defense upgrade
        upgradeSlots[2].Init("Defense +1", "Reduce damage taken", defenseIcon, 60, ShopItem.CurrencyType.Souls, () =>
        {
            playerData.baseDefense += 1;
            playerData.AddUpgrade(DEFENSE_UPGRADE);
            DisableUpgradeButton(upgradeSlots[2]);
        });

        // Check if upgrades are already purchased
        CheckPurchasedUpgrades();
    }

    private void CheckPurchasedUpgrades()
    {
        var playerData = GameManager.Instance.PlayerData;

        if (playerData.HasUpgrade(HEALTH_UPGRADE))
            DisableUpgradeButton(upgradeSlots[0]);

        if (playerData.HasUpgrade(DAMAGE_UPGRADE))
            DisableUpgradeButton(upgradeSlots[1]);

        if (playerData.HasUpgrade(DEFENSE_UPGRADE))
            DisableUpgradeButton(upgradeSlots[2]);
    }

    private void DisableUpgradeButton(ShopItem item)
    {
        item.buyButton.interactable = false;
        
        if (item.itemPrice != null)
            item.itemPrice.text = "PURCHASED";
    }
} 