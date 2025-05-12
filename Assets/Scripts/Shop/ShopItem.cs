using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShopItem : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text itemPrice;
    public Button buyButton;
    public Image currencyIcon; // Image for gold/souls icon

    [Header("Currency Icons")]
    public Sprite goldIcon;
    public Sprite soulsIcon;

    public enum CurrencyType { Gold, Souls }
    public CurrencyType currency;
    public int price;

    public UnityAction onPurchase;

    private void Awake()
    {
        buyButton.onClick.AddListener(TryPurchase);
    }

    private void Update()
    {
        // Check if we can afford this item
        UpdateBuyButtonState();
    }

    public void Init(string name, string desc, Sprite icon, int price, CurrencyType currency, UnityAction onBuy)
    {
        itemName.text = name;
        itemDescription.text = desc;
        itemIcon.sprite = icon;
        this.price = price;
        this.currency = currency;
        this.onPurchase = onBuy;

        itemPrice.text = price.ToString();

        // Set currency icon
        if (currencyIcon != null)
            currencyIcon.sprite = currency == CurrencyType.Gold ? goldIcon : soulsIcon;

        // Initial button state
        UpdateBuyButtonState();
    }

    private void UpdateBuyButtonState()
    {
        var data = GameManager.Instance.PlayerData;
        bool canAfford = (currency == CurrencyType.Gold && data.gold >= price) ||
                         (currency == CurrencyType.Souls && data.souls >= price);
        
        buyButton.interactable = canAfford;
    }

    private void TryPurchase()
    {
        var data = GameManager.Instance.PlayerData;

        if ((currency == CurrencyType.Gold && data.gold >= price) ||
            (currency == CurrencyType.Souls && data.souls >= price))
        {
            if (currency == CurrencyType.Gold)
                data.AddGold(-price);
            else
                data.AddSouls(-price);

            onPurchase?.Invoke();
        }
    }
}
