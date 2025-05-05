using SQLite;

public class ShopItemData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public string itemID { get; set; }
    public string itemName { get; set; }
    public string itemDescription { get; set; }
    public int price { get; set; }
    public string currency { get; set; } // "Gold" or "Souls"
}