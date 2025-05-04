using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
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
    
    private void Start()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(false);
        
        // Setup default rewards if none are defined
        if (possibleRewards.Count == 0)
            SetupDefaultRewards();
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
    }
    
    public void ShowRewards()
    {
        GenerateRewards();
        DisplayRewards();
        
        if (rewardPanel != null)
            rewardPanel.SetActive(true);
    }
    
    private void GenerateRewards()
    {
        currentRewards.Clear();

        List<Reward> shuffled = new List<Reward>(possibleRewards);
        Shuffle(shuffled);

        int rewardCount = Mathf.Min(rewardsToChoose, rewardSlots.Length, shuffled.Count);
        currentRewards.AddRange(shuffled.GetRange(0, rewardCount));
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
    private void UpdateRewardUI(int index, Reward reward)
    {
        if (rewardImages.Length > index && rewardImages[index] != null)
            rewardImages[index].sprite = reward.icon;

        if (rewardTitles.Length > index && rewardTitles[index] != null)
            rewardTitles[index].text = reward.rewardName;

        if (rewardValues.Length > index && rewardValues[index] != null)
            rewardValues[index].text = "+" + reward.value.ToString();

        if (claimButtons.Length > index && claimButtons[index] != null)
        {
            claimButtons[index].onClick.RemoveAllListeners();
            claimButtons[index].onClick.AddListener(() => ClaimReward(index));
        }

        rewardSlots[index].SetActive(true);
    }


    private void DisplayRewards()
    {
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i < currentRewards.Count)
                UpdateRewardUI(i, currentRewards[i]);
            else
                rewardSlots[i].SetActive(false);
        }
    }
    
    public void ClaimReward(int index)
    {
        Debug.Log($"Clicked reward index: {index}");
        if (index >= 0 && index < currentRewards.Count)
        {
            Reward selectedReward = currentRewards[index];
            ApplyReward(selectedReward);

            rewardPanel?.SetActive(false);

            MapSystem mapSystem = FindFirstObjectByType<MapSystem>();
            mapSystem?.SaveMapState();

            GameManager.Instance?.ChangeState(GameState.Map);

        }
    }
    
    private void ApplyReward(Reward reward)
    {
        GameManager.Instance?.PlayerData?.ApplyReward(reward);
    }
}