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
        }
    }

    public void SavePlayerUpgrade(List<string> upgrades)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.DeleteAll<PlayerUpgrade>();
            
            foreach (string id in upgrades)
            {
                connection.Insert(new PlayerUpgrade { upgradeID = id });
            }
        }
    }

    public List<string> LoadPlayerUpgrade()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            var upgrades = connection.Table<PlayerUpgrade>().ToList();
            return upgrades.Select(u => u.upgradeID).ToList();
        }
    }

    public void SaveShopItems(List<ShopItemData> items)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<ShopItemData>();
            
            foreach (var item in items)
                connection.InsertOrReplace(item);
        }
    }

    public List<ShopItemData> LoadShopItems()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<ShopItemData>();
            return connection.Table<ShopItemData>().ToList();
        }
    }
}
