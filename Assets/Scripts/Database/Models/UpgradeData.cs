using SQLite;

public class UpgradeData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    
    public string upgradeID { get; set; }
    public string upgradeName { get; set; }
    public string upgradeDescription { get; set; }
    public int price { get; set; }
    public string currency { get; set; }
}