using SQLite;

public class PlayerUpgrade
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string upgradeID { get; set; }
}