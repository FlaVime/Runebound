using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Item Icons")]
    public Sprite potionIcon;
    public Sprite greaterPotionIcon;
    public Sprite soulStoneIcon;

    public ShopItem[] itemSlots;

    private void Start()
    {
        // Health Potion
        itemSlots[0].Init("Health Potion", "Restore 30 HP", potionIcon, 15, ShopItem.CurrencyType.Gold, () =>
        {
            GameManager.Instance.PlayerData.Heal(30);
        });

        // Energy Potion
        itemSlots[1].Init("Greater Potion", "Restore 50 HP", greaterPotionIcon, 30, ShopItem.CurrencyType.Gold, () =>
        {
            GameManager.Instance.PlayerData.Heal(50);
        });

        // Soul Stone
        itemSlots[2].Init("Soul Stone", "Get 5 souls", soulStoneIcon, 50, ShopItem.CurrencyType.Gold, () =>
        {
            GameManager.Instance.PlayerData.AddSouls(5);
        });

        // Big Soul Stone
        itemSlots[3].Init("Big Soul Stone", "Get 25 souls", soulStoneIcon, 100, ShopItem.CurrencyType.Gold, () =>
        {
            GameManager.Instance.PlayerData.AddSouls(25);
        });
    }
}
