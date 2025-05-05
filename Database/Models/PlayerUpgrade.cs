using SQLite;

public class PlayerUpgrade
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [Indexed] // This will create an index on the upgradeID column for faster lookups
    public string upgradeID { get; set; }
}