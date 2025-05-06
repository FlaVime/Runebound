using System.Collections.Generic;
using System.Linq;
using SQLite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private string dbPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else Destroy(gameObject);
    }

    private void InitializeDatabase()
    {
        dbPath = System.IO.Path.Combine(Application.persistentDataPath, "GameDatabase.db");
        Debug.Log("Database Path: " + dbPath);

        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<PlayerUpgrade>();
            connection.CreateTable<ShopItemData>();
            connection.CreateTable<UpgradeData>();
        }

        SeedDataIfEmpty();
    }

    private void SeedDataIfEmpty()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            // SHOP ITEMS
            var existingItems = connection.Table<ShopItemData>().ToList();
            if (existingItems.Count == 0)
            {
                var shopItems = new List<ShopItemData>
                {
                    new ShopItemData { itemID = CryptoUtils.Encrypt("potion_30"), itemName = CryptoUtils.Encrypt("Health Potion"), itemDescription = CryptoUtils.Encrypt("Restore 30 HP"), price = 15, currency = CryptoUtils.Encrypt("Gold") },
                    new ShopItemData { itemID = CryptoUtils.Encrypt("potion_50"), itemName = CryptoUtils.Encrypt("Greater Potion"), itemDescription = CryptoUtils.Encrypt("Restore 50 HP"), price = 30, currency = CryptoUtils.Encrypt("Gold") },
                    new ShopItemData { itemID = CryptoUtils.Encrypt("soul_5"), itemName = CryptoUtils.Encrypt("Soul Stone"), itemDescription = CryptoUtils.Encrypt("Get 5 Souls"), price = 50, currency = CryptoUtils.Encrypt("Gold") },
                    new ShopItemData { itemID = CryptoUtils.Encrypt("soul_25"), itemName = CryptoUtils.Encrypt("Big Soul Stone"), itemDescription = CryptoUtils.Encrypt("Get 25 Souls"), price = 100, currency = CryptoUtils.Encrypt("Gold") }
                };

                foreach (var item in shopItems)
                    connection.Insert(item);

                Debug.Log("Seeded ShopItemData");
            }

            // UPGRADES
            var existingUpgrades = connection.Table<UpgradeData>().ToList();
            if (existingUpgrades.Count == 0)
            {
                var upgrades = new List<UpgradeData>
                {
                    new UpgradeData { upgradeID = CryptoUtils.Encrypt("max_health"), upgradeName = CryptoUtils.Encrypt("Max Health + 20"), upgradeDescription = CryptoUtils.Encrypt("Permanently increase max HP"), price = 50, currency = CryptoUtils.Encrypt("Souls") },
                    new UpgradeData { upgradeID = CryptoUtils.Encrypt("damage_boost"), upgradeName = CryptoUtils.Encrypt("Base Damage + 2"), upgradeDescription = CryptoUtils.Encrypt("Increase your attack power"), price = 75, currency = CryptoUtils.Encrypt("Souls") },
                    new UpgradeData { upgradeID = CryptoUtils.Encrypt("defense_boost"), upgradeName = CryptoUtils.Encrypt("Defence + 1"), upgradeDescription = CryptoUtils.Encrypt("Reduce damage taken"), price = 60, currency = CryptoUtils.Encrypt("Souls") },
                    new UpgradeData { upgradeID = CryptoUtils.Encrypt("max_energy"), upgradeName = CryptoUtils.Encrypt("Max Energy + 1"), upgradeDescription = CryptoUtils.Encrypt("Increase your max energy"), price = 70, currency = CryptoUtils.Encrypt("Souls") },
                };

                foreach (var upgrade in upgrades)
                    connection.Insert(upgrade);

                Debug.Log("Seeded UpgradeData");
            }
        }
    }

    public void SavePlayerUpgrade(List<string> upgrades)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.DeleteAll<PlayerUpgrade>();
            
            foreach (string id in upgrades)
            {
                string encryptedId = CryptoUtils.Encrypt(id); // Encrypt the ID before saving
                connection.Insert(new PlayerUpgrade { upgradeID = encryptedId });
            }
        }
    }

    public List<string> LoadPlayerUpgrade()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            var upgrades = connection.Table<PlayerUpgrade>().ToList();
            return upgrades.Select(u => CryptoUtils.Decrypt(u.upgradeID)).ToList();
        }
    }

    public void SaveShopItems(List<ShopItemData> items)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<ShopItemData>();
            
            foreach (var item in items)
            {
                var encryptedItem = new ShopItemData
                {
                    id = item.id,
                    itemID = CryptoUtils.Encrypt(item.itemID), // Encrypt the itemID before saving
                    itemName = CryptoUtils.Encrypt(item.itemName),
                    itemDescription = CryptoUtils.Encrypt(item.itemDescription),
                    price = item.price,
                    currency = CryptoUtils.Encrypt(item.currency)
                };

                connection.InsertOrReplace(encryptedItem);
            }
        }
    }

    public List<ShopItemData> LoadShopItems()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<ShopItemData>();
            var encryptedItems = connection.Table<ShopItemData>().ToList();

            var decryptedItems = encryptedItems.Select(item => new ShopItemData
            {
                id = item.id,
                itemID = CryptoUtils.Decrypt(item.itemID),
                itemName = CryptoUtils.Decrypt(item.itemName),
                itemDescription = CryptoUtils.Decrypt(item.itemDescription),
                price = item.price,
                currency = CryptoUtils.Decrypt(item.currency)
            }).ToList();

            return decryptedItems;
        }
    }

    public List<UpgradeData> LoadUpgrades()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<UpgradeData>();
            var encryptedUpgrades = connection.Table<UpgradeData>().ToList();

            var decryptedUpgrades = encryptedUpgrades.Select(u => new UpgradeData
            {
                id = u.id,
                upgradeID = CryptoUtils.Decrypt(u.upgradeID),
                upgradeName = CryptoUtils.Decrypt(u.upgradeName),
                upgradeDescription = CryptoUtils.Decrypt(u.upgradeDescription),
                price = u.price,
                currency = CryptoUtils.Decrypt(u.currency)
            }).ToList();

            return decryptedUpgrades;
        }
    }
}
