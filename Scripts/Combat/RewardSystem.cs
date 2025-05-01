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
        public string description;
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
    public Button[] rewardButtons;
    public TMP_Text[] rewardTexts;
    public Image[] rewardIcons;
    
    [Header("Reward Settings")]
    public List<Reward> possibleRewards = new List<Reward>();
    public int rewardsToChoose = 3;
    
    private List<Reward> currentRewards = new List<Reward>();
    
    private void Start()
    {
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
    }
    
    public void ShowRewards()
    {
        GenerateRewards();
        DisplayRewards();
        
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
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
        int rewardCount = Mathf.Min(rewardsToChoose, rewardButtons.Length);
        
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
    }
    
    private void DisplayRewards()
    {
        // Display each reward on the corresponding button
        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < currentRewards.Count)
            {
                // Enable the button
                rewardButtons[i].gameObject.SetActive(true);
                
                // Set the reward icon
                if (rewardIcons.Length > i && rewardIcons[i] != null)
                {
                    rewardIcons[i].sprite = currentRewards[i].icon;
                    rewardIcons[i].gameObject.SetActive(true);
                }
                
                // Set the reward text
                if (rewardTexts.Length > i && rewardTexts[i] != null)
                {
                    rewardTexts[i].text = currentRewards[i].rewardName;
                }
                
                // Setup button click event
                int index = i; // Need to capture the index for the delegate
                rewardButtons[i].onClick.RemoveAllListeners();
                rewardButtons[i].onClick.AddListener(() => ClaimReward(index));
            }
            else
            {
                // Disable extra buttons
                rewardButtons[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void ClaimReward(int index)
    {
        if (index >= 0 && index < currentRewards.Count)
        {
            Reward selectedReward = currentRewards[index];
            ApplyReward(selectedReward);
            
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
    }
    
    private void ApplyReward(Reward reward)
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerData == null)
            return;
            
        switch (reward.type)
        {
            case RewardType.Gold:
                GameManager.Instance.PlayerData.AddGold(reward.value);
                break;
                
            case RewardType.Souls:
                GameManager.Instance.PlayerData.AddSouls(reward.value);
                break;
                
            case RewardType.Health:
                GameManager.Instance.PlayerData.Heal(reward.value);
                break;
                
            case RewardType.MaxHealth:
                GameManager.Instance.PlayerData.maxHealth += reward.value;
                GameManager.Instance.PlayerData.Heal(reward.value); // Heal by the amount increased
                break;
                
            case RewardType.MaxEnergy:
                GameManager.Instance.PlayerData.maxEnergy += reward.value;
                break;
        }
    }
} 