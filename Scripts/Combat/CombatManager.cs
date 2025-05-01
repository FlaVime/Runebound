using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("References")]
    public PlayerController player;
    public List<Enemy> enemies;
    
    [Header("Turn Settings")]
    public float enemyTurnDelay = 1f;
    
    [Header("UI References")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public GameObject rewardPanel;
    public Button[] rewardButtons;
    
    public UnityEvent onGameOver;
    public UnityEvent onVictory;
    public UnityEvent<CombatState> onStateChanged;

    private CombatState currentState;
    public CombatState CurrentState => currentState;
    
    private int combatGoldReward;
    private int combatSoulsReward;
    private int combatExperienceReward;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize UI
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }

    private void Start()
    {
        SetupCombat();
    }

    private void SetupCombat()
    {
        // Reset accumulated rewards
        combatGoldReward = 0;
        combatSoulsReward = 0;
        combatExperienceReward = 0;
        
        // Setup enemy death listeners
        foreach (var enemy in enemies)
        {
            enemy.onDeath.AddListener(() => CheckVictoryCondition());
            enemy.onEnemyDefeated.AddListener(() => AccumulateRewards(enemy));
        }
        
        // Setup player death
        player.onDeath.AddListener(GameOver);
        
        // Start with player turn
        SetState(CombatState.PlayerTurn);
    }
    
    private void AccumulateRewards(Enemy enemy)
    {
        combatGoldReward += enemy.goldReward;
        combatSoulsReward += enemy.soulsReward;
        combatExperienceReward += enemy.experienceReward;
    }

    public void SetState(CombatState newState)
    {
        currentState = newState;
        onStateChanged?.Invoke(newState);

        switch (newState)
        {
            case CombatState.PlayerTurn:
                StartPlayerTurn();
                break;
            case CombatState.EnemyTurn:
                StartCoroutine(EnemyTurnSequence());
                break;
            case CombatState.Victory:
                HandleVictory();
                break;
            case CombatState.GameOver:
                HandleGameOver();
                break;
        }
    }

    private void StartPlayerTurn()
    {
        // Reset any turn-based effects here
    }

    private IEnumerator EnemyTurnSequence()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                enemy.TakeTurn();
                yield return new WaitForSeconds(enemyTurnDelay);
            }
        }

        SetState(CombatState.PlayerTurn);
    }

    private void CheckVictoryCondition()
    {
        bool allEnemiesDead = true;
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                allEnemiesDead = false;
                break;
            }
        }

        if (allEnemiesDead)
        {
            Victory();
        }
    }

    private void Victory()
    {
        SetState(CombatState.Victory);
        onVictory?.Invoke();
    }
    
    private void HandleVictory()
    {
        // Show victory UI
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        // Setup reward panel if available
        SetupRewardOptions();
        
        // Update GameManager state
        if (GameManager.Instance != null)
        {
            // Don't transition yet - wait for reward selection
        }
    }
    
    private void SetupRewardOptions()
    {
        // Show reward panel after a short delay
        StartCoroutine(ShowRewardPanel());
    }
    
    private IEnumerator ShowRewardPanel()
    {
        yield return new WaitForSeconds(1.5f);
        
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
            
            // Setup reward buttons with random rewards
            if (rewardButtons != null && rewardButtons.Length > 0)
            {
                // Setup rewards (this is just an example - expand as needed)
                SetupRewardButton(rewardButtons[0], "Gold", () => GiveGoldReward());
                
                if (rewardButtons.Length > 1)
                {
                    SetupRewardButton(rewardButtons[1], "Souls", () => GiveSoulsReward());
                }
                
                if (rewardButtons.Length > 2)
                {
                    SetupRewardButton(rewardButtons[2], "Health", () => GiveHealthReward());
                }
            }
        }
    }
    
    private void SetupRewardButton(Button button, string rewardType, UnityAction action)
    {
        if (button != null)
        {
            // Clear previous listeners
            button.onClick.RemoveAllListeners();
            
            // Set button text
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = rewardType;
            }
            
            // Add listener
            button.onClick.AddListener(action);
            button.onClick.AddListener(ReturnToMap);
        }
    }
    
    private void GiveGoldReward()
    {
        // Give bonus gold (50% more)
        int bonusGold = Mathf.FloorToInt(combatGoldReward * 0.5f);
        GameManager.Instance.GiveRewards(combatGoldReward + bonusGold, combatSoulsReward, combatExperienceReward);
    }
    
    private void GiveSoulsReward()
    {
        // Give bonus souls (50% more)
        int bonusSouls = Mathf.FloorToInt(combatSoulsReward * 0.5f);
        GameManager.Instance.GiveRewards(combatGoldReward, combatSoulsReward + bonusSouls, combatExperienceReward);
    }
    
    private void GiveHealthReward()
    {
        // Give standard rewards plus heal
        GameManager.Instance.GiveRewards(combatGoldReward, combatSoulsReward, combatExperienceReward);
        GameManager.Instance.PlayerData.Heal(20);
    }
    
    private void ReturnToMap()
    {
        // Return to map
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.Map);
        }
    }

    private void GameOver()
    {
        SetState(CombatState.GameOver);
        onGameOver?.Invoke();
    }
    
    private void HandleGameOver()
    {
        // Show game over UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        // Update GameManager state after a delay
        StartCoroutine(GameOverDelay());
    }
    
    private IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(2.0f);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
    }
    
    // UI Button methods (to be called from buttons)
    public void RetryButton()
    {
        // Reload combat scene
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.Combat);
        }
    }
    
    public void MainMenuButton()
    {
        // Return to main menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
} 