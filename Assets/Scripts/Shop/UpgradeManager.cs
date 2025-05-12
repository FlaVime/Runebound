using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public Sprite healthIcon;
    public Sprite damageIcon;
    public Sprite defenseIcon;
    public Sprite energyIcon;

    public ShopItem[] upgradeSlots;

    private const string HEALTH_UPGRADE = "max_health";
    private const string DAMAGE_UPGRADE = "damage_boost";  
    private const string DEFENSE_UPGRADE = "defense_boost";
    private const string ENERGY_UPGRADE = "max_energy";

    private void Start()
    {
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        var playerData = GameManager.Instance.PlayerData;
        var upgrades = DatabaseManager.Instance.LoadUpgrades();

        for (int i = 0; i < upgradeSlots.Length && i < upgrades.Count; i++)
        {
            var upgradeData = upgrades[i];
            ShopItem.CurrencyType currencyType = upgradeData.currency == "Gold" ? ShopItem.CurrencyType.Gold : ShopItem.CurrencyType.Souls;

            upgradeSlots[i].Init(upgradeData.upgradeName, upgradeData.upgradeDescription, GetIconByID(upgradeData.upgradeID), upgradeData.price, currencyType, () =>
            {
                ApplyUpgradeEffect(upgradeData.upgradeID);
                playerData.AddUpgrade(upgradeData.upgradeID);
            });
        }
    }

    private Sprite GetIconByID(string upgradeID)
    {
        if (upgradeID == HEALTH_UPGRADE) return healthIcon;
        if (upgradeID == DAMAGE_UPGRADE) return damageIcon;
        if (upgradeID == DEFENSE_UPGRADE) return defenseIcon;
        if (upgradeID == ENERGY_UPGRADE) return energyIcon;
        return null; // Default icon or handle error
    }

    private void ApplyUpgradeEffect(string upgradeID)
    {
        var playerData = GameManager.Instance.PlayerData;

        switch (upgradeID)
        {
            case "max_health":
                playerData.maxHealth += 20;
                playerData.Heal(20);
                break;
            case "damage_boost":
                playerData.baseDamage += 2;
                break;
            case "defense_boost":
                playerData.baseDefense += 1;
                break;
            case "max_energy":
                playerData.maxEnergy += 1;
                playerData.AddEnergy(1);
                break;
        }
    }
} 