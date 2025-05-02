using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardSystem : MonoBehaviour
{
    [System.Serializable]
    public class Reward
    {
        public string rewardName;
        public Sprite icon;
        public RewardType type;
        public int value;
    }
    
    public enum RewardType
    {
        Gold,
        Souls,
        Health,
        MaxHealth,
        MaxEnergy
    }

    [Header("UI References")]
    public GameObject rewardPanel;
    public GameObject[] rewardSlots; // Parent objects containing image, text, description
    public Image[] rewardImages;
    public TMP_Text[] rewardTitles;
    public TMP_Text[] rewardValues;
    public Button[] claimButtons;
    
    [Header("Reward Settings")]
    public List<Reward> possibleRewards = new List<Reward>();
    public int rewardsToChoose = 3;
    
    [Header("Default Rewards")]
    public Sprite goldIcon;
    public Sprite soulsIcon;
    public Sprite healthIcon;
    
    private List<Reward> currentRewards = new List<Reward>();
    
    private void OnEnable()
    {
        // Log that the RewardSystem has been enabled
        Debug.Log("RewardSystem script enabled");
        
        // Run a validation check
        ValidateSetup();
    }
    
    private void Start()
    {
        Debug.Log("RewardSystem Start() called");
        
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("RewardSystem: rewardPanel reference is missing!");
        }
        
        // Setup default rewards if none are defined
        if (possibleRewards.Count == 0)
        {
            Debug.Log("No rewards defined, setting up defaults");
            SetupDefaultRewards();
        }
    }
    
    private void ValidateSetup()
    {
        // Check if reference arrays are set up
        if (rewardPanel == null)
            Debug.LogError("RewardSystem: rewardPanel is null!");
            
        if (rewardSlots == null || rewardSlots.Length == 0)
            Debug.LogError("RewardSystem: rewardSlots array is empty!");
            
        if (rewardImages == null || rewardImages.Length == 0)
            Debug.LogError("RewardSystem: rewardImages array is empty!");
            
        if (rewardTitles == null || rewardTitles.Length == 0)
            Debug.LogError("RewardSystem: rewardTitles array is empty!");
            
        if (rewardValues == null || rewardValues.Length == 0)
            Debug.LogError("RewardSystem: rewardValues array is empty!");
            
        if (claimButtons == null || claimButtons.Length == 0)
            Debug.LogError("RewardSystem: claimButtons array is empty!");
            
        // Check default sprites
        if (goldIcon == null)
            Debug.LogError("RewardSystem: goldIcon is missing!");
            
        if (soulsIcon == null)
            Debug.LogError("RewardSystem: soulsIcon is missing!");
            
        if (healthIcon == null)
            Debug.LogError("RewardSystem: healthIcon is missing!");
    }
    
    private void SetupDefaultRewards()
    {
        // Gold reward
        Reward goldReward = new Reward
        {
            rewardName = "Gold",
            icon = goldIcon,
            type = RewardType.Gold,
            value = 25
        };
        
        // Souls reward
        Reward soulsReward = new Reward
        {
            rewardName = "Souls",
            icon = soulsIcon,
            type = RewardType.Souls,
            value = 15
        };
        
        // Health reward
        Reward healthReward = new Reward
        {
            rewardName = "Health",
            icon = healthIcon,
            type = RewardType.Health,
            value = 20
        };
        
        // Add to possible rewards
        possibleRewards.Add(goldReward);
        possibleRewards.Add(soulsReward);
        possibleRewards.Add(healthReward);
        
        Debug.Log("Added default rewards: " + possibleRewards.Count);
    }
    
    public void ShowRewards()
    {
        Debug.Log("ShowRewards called");
        
        GenerateRewards();
        DisplayRewards();
        
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
            Debug.Log("Reward panel activated");
        }
        else
        {
            Debug.LogError("Cannot show rewards - rewardPanel is null!");
        }
    }
    
    private void GenerateRewards()
    {
        // Clear previous rewards
        currentRewards.Clear();
        
        // Ensure we have enough possible rewards
        if (possibleRewards.Count == 0)
        {
            Debug.LogError("No possible rewards defined!");
            return;
        }
        
        // Determine how many rewards to generate
        int rewardCount = Mathf.Min(rewardsToChoose, rewardSlots.Length);
        
        // Generate random rewards
        List<int> usedIndices = new List<int>();
        
        for (int i = 0; i < rewardCount; i++)
        {
            // Generate a unique reward index
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, possibleRewards.Count);
            } while (usedIndices.Contains(randomIndex) && usedIndices.Count < possibleRewards.Count);
            
            usedIndices.Add(randomIndex);
            currentRewards.Add(possibleRewards[randomIndex]);
        }
        
        Debug.Log("Generated " + currentRewards.Count + " rewards");
    }
    
    private void DisplayRewards()
    {
        Debug.Log("DisplayRewards called, slots: " + rewardSlots.Length + ", current rewards: " + currentRewards.Count);
        
        // Display each reward on the corresponding button
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i < currentRewards.Count)
            {
                // Enable the reward slot
                rewardSlots[i].SetActive(true);
                
                // Set the reward image
                if (rewardImages.Length > i && rewardImages[i] != null)
                {
                    rewardImages[i].sprite = currentRewards[i].icon;
                    rewardImages[i].gameObject.SetActive(true);
                    Debug.Log("Set reward image " + i + " to " + currentRewards[i].rewardName);
                }
                else
                {
                    Debug.LogError("RewardImage " + i + " is missing or array is too short");
                }
                
                // Set the reward title
                if (rewardTitles.Length > i && rewardTitles[i] != null)
                {
                    rewardTitles[i].text = currentRewards[i].rewardName;
                    Debug.Log("Set reward title " + i + " to " + currentRewards[i].rewardName);
                }
                else
                {
                    Debug.LogError("RewardTitle " + i + " is missing or array is too short");
                }
                
                // Set the reward value
                if (rewardValues.Length > i && rewardValues[i] != null)
                {
                    rewardValues[i].text = "+" + currentRewards[i].value.ToString();
                    Debug.Log("Set reward value " + i + " to +" + currentRewards[i].value);
                }
                else
                {
                    Debug.LogError("RewardValue " + i + " is missing or array is too short");
                }
                
                // Setup claim button click event
                if (claimButtons.Length > i && claimButtons[i] != null)
                {
                    int index = i; // Need to capture the index for the delegate
                    claimButtons[i].onClick.RemoveAllListeners();
                    claimButtons[i].onClick.AddListener(() => ClaimReward(index));
                    Debug.Log("Set up claim button " + i);
                }
                else
                {
                    Debug.LogError("ClaimButton " + i + " is missing or array is too short");
                }
            }
            else
            {
                // Disable extra slots
                rewardSlots[i].SetActive(false);
            }
        }
    }
    
    public void ClaimReward(int index)
    {
        Debug.Log("ClaimReward called for index " + index);
        if (index >= 0 && index < currentRewards.Count)
        {
            Reward selectedReward = currentRewards[index];
            ApplyReward(selectedReward);
            
            // Play reward claim animation or sound
            StartCoroutine(PlayRewardAnimation(index));
        }
        else
        {
            Debug.LogError("Invalid reward index: " + index);
        }
    }
    
    private IEnumerator PlayRewardAnimation(int index)
    {
        // Simple flash animation
        if (rewardImages.Length > index && rewardImages[index] != null)
        {
            // Save original color
            Color originalColor = rewardImages[index].color;
            
            // Flash bright
            rewardImages[index].color = Color.white;
            yield return new WaitForSeconds(0.2f);
            
            // Return to original color
            rewardImages[index].color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
        
        // Close reward panel
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
        
        // Return to map
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.Map);
        }
    }
    
    private void ApplyReward(Reward reward)
    {
        Debug.Log("Applying reward: " + reward.rewardName + " of type " + reward.type + " with value " + reward.value);
        
        if (GameManager.Instance == null || GameManager.Instance.PlayerData == null)
        {
            Debug.LogError("GameManager or PlayerData is null, cannot apply reward!");
            return;
        }
            
        switch (reward.type)
        {
            case RewardType.Gold:
                GameManager.Instance.PlayerData.AddGold(reward.value);
                Debug.Log("Added " + reward.value + " gold");
                break;
                
            case RewardType.Souls:
                GameManager.Instance.PlayerData.AddSouls(reward.value);
                Debug.Log("Added " + reward.value + " souls");
                break;
                
            case RewardType.Health:
                GameManager.Instance.PlayerData.Heal(reward.value);
                Debug.Log("Healed " + reward.value + " health");
                break;
                
            case RewardType.MaxHealth:
                GameManager.Instance.PlayerData.maxHealth += reward.value;
                GameManager.Instance.PlayerData.Heal(reward.value); // Heal by the amount increased
                Debug.Log("Increased max health by " + reward.value);
                break;
                
            case RewardType.MaxEnergy:
                GameManager.Instance.PlayerData.maxEnergy += reward.value;
                Debug.Log("Increased max energy by " + reward.value);
                break;
        }
    }
    
    // Public method to test the reward system directly from the inspector or other scripts
    public void TestRewards()
    {
        Debug.Log("Test rewards called");
        ShowRewards();
    }
} 