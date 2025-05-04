using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour
{
    [Header("Screens")]
    public GameObject shopScreen;
    public GameObject upgradeScreen;
    
    [Header("Top Bar")]
    public TextMeshProUGUI shopTitleText;
    public Button shopButton;
    public Button upgradeButton;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI soulsText;
    public TextMeshProUGUI runesText;
    
    [Header("Managers")]
    public ShopManager shopManager;
    public UpgradeManager upgradeManager;
    
    [Header("Exit")]
    public Button exitButton;
    
    private void Start()
    {
        // Set up button listeners
        shopButton.onClick.AddListener(ShowShopScreen);
        upgradeButton.onClick.AddListener(ShowUpgradeScreen);
        exitButton.onClick.AddListener(ExitShop);
        
        // Show shop screen by default
        ShowShopScreen();
    }
    
    public void ShowShopScreen()
    {
        shopTitleText.text = "Shop";
        shopScreen.SetActive(true);
        upgradeScreen.SetActive(false);
        
        // Visual indication of selected tab
        shopButton.interactable = false;
        upgradeButton.interactable = true;
    }
    
    public void ShowUpgradeScreen()
    {
        shopTitleText.text = "Upgrades";
        shopScreen.SetActive(false);
        upgradeScreen.SetActive(true);
        
        // Visual indication of selected tab
        shopButton.interactable = true;
        upgradeButton.interactable = false;
    }
    
    public void ExitShop()
    {
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene("Map");
    }
} 