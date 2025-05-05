using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Item Icons")]
    public Sprite potionIcon;
    public Sprite greaterPotionIcon;
    public Sprite soulStoneIcon;

    public ShopItem[] itemSlots;

    private void Start()
    {
        List<ShopItemData> items = DatabaseManager.Instance.LoadShopItems();

        if (items == null || items.Count == 0)
        {
            Debug.LogError("No shop items found in the database.");
            return;
        }

        for (int i = 0; i < itemSlots.Length && i < items.Count; i++)
        {
            ShopItemData itemData = items[i];
            Sprite itemIcon = GetIconByID(itemData.itemID);
            ShopItem.CurrencyType currencyType = itemData.currency == "Gold" ? ShopItem.CurrencyType.Gold : ShopItem.CurrencyType.Souls;

            itemSlots[i].Init(itemData.itemName, itemData.itemDescription, itemIcon, itemData.price, currencyType, () =>
            {
                ApplyItemEffect(itemData.itemID);
            });
        }
    }

    private Sprite GetIconByID(string itemID)
    {
        if (itemID.Contains("30")) return potionIcon;
        if (itemID.Contains("50")) return greaterPotionIcon;
        return soulStoneIcon;
    }

    private void ApplyItemEffect(string itemID)
    {
        var itemData = GameManager.Instance.PlayerData;
        
        switch (itemID)
        {
            case "potion_30": itemData.Heal(30); break;
            case "potion_50": itemData.Heal(50); break;
            case "soul_5": itemData.AddSouls(5); break;
            case "soul_25": itemData.AddSouls(25); break;
        }
    }
}
