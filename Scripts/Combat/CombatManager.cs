using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CombatState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Setup")]
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs; // List of different enemy prefabs
    public GameObject defaultEnemyPrefab; // Default enemy if no specific one is provided
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    [Header("Units")]
    CharacterBase playerUnit;
    CharacterBase enemyUnit;
    private Enemy currentEnemy; // Reference to the current enemy as Enemy class

    [Header("Energy Management")]
    public int maxEnergy = 3;
    public int currentEnergy = 3;
    public int attackEnergyCost = 1;
    public int defenseEnergyCost = 1;
    public int abilityCost = 2;
    public int energyPerTurn = 1;
    
    [Header("Multipliers")]
    public float abilityDamageMultiplier = 1.5f;
    public float defendMultiplier = 0.5f;
    public float skipMultiplier = 1.5f;


    [Header("UI")]
    public CombatHUD playerHUD;
    public CombatHUD enemyHUD;
    public GameObject actionButtonsPanel;
    public GameObject rewardPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public TMP_Text energyText;
    public Button attackButton;
    public Button defenseButton;
    public Button abilityButton;
    public GameObject combatArea;
    
    [Header("Background")]
    public BackgroundManager backgroundManager;
    
    [Header("Rewards")]
    public RewardSystem rewardSystem;

    public CombatState combatState;
    
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
        
        if (rewardSystem == null)
        {
            rewardSystem = FindFirstObjectByType<RewardSystem>();
        }
        
        if (backgroundManager == null) backgroundManager = FindFirstObjectByType<BackgroundManager>();
        
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }
    
    void Start()
    {
        combatState = CombatState.Start;
        SetupCombat();
    }

    public void SetupCombat()
    {
        if (combatArea != null) combatArea.SetActive(true);
        
        if (backgroundManager != null) backgroundManager.SetRandomBackground();        
        
        // Reset energy
        currentEnergy = maxEnergy;
        UpdateEnergyDisplay();
        
        // Spawn player character
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<CharacterBase>();
        
        if (GameManager.Instance != null)
        {
            float healthPercentage = (float)GameManager.Instance.PlayerData.currentHealth / GameManager.Instance.PlayerData.maxHealth;
            playerUnit.currentHealth = playerUnit.maxHealth * healthPercentage;
        }

        playerHUD.SetHUD(playerUnit);
        playerHUD.SetHealth(playerUnit.currentHealth);
        
        SpawnRandomEnemy();
        
        // Subscribe to health change events
        playerUnit.onHealthChanged.AddListener(OnPlayerHealthChanged);
        enemyUnit.onHealthChanged.AddListener(OnEnemyHealthChanged);
        
        // Start player turn
        StartCoroutine(StartPlayerTurn());
    }
    
    private void SpawnRandomEnemy()
    {
        GameObject enemyPrefab;
        
        if (enemyPrefabs != null && enemyPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Count);
            enemyPrefab = enemyPrefabs[randomIndex];
        }
        else enemyPrefab = defaultEnemyPrefab;
        
        // Create the enemy
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<CharacterBase>();
        
        // Try to get the Enemy component if available
        currentEnemy = enemyGO.GetComponent<Enemy>();
        
        // Make sure health is initialized
        if (enemyUnit.currentHealth <= 0)
        {
            enemyUnit.currentHealth = enemyUnit.maxHealth;
        }
        
        // Make sure enemy name is properly set in UI
        if (enemyHUD != null)
        {
            // Setting HUD will also set up the health bar
            enemyHUD.SetHUD(enemyUnit);
            
            // Explicitly update the health bar with current value
            enemyHUD.SetHealth(enemyUnit.currentHealth);
        }
    }
    
    private void OnPlayerHealthChanged(float healthPercent)
    {
        playerHUD.SetHealth(playerUnit.currentHealth);
        
        if (healthPercent <= 0)
        {
            combatState = CombatState.Lost;
            EndCombat();
        }
    }
    
    private void OnEnemyHealthChanged(float healthPercent)
    {
        enemyHUD.SetHealth(enemyUnit.currentHealth);
        
        if (healthPercent <= 0)
        {
            combatState = CombatState.Won;
            EndCombat();
        }
    }
    
    private IEnumerator StartPlayerTurn()
    {
        yield return new WaitForSeconds(2.0f);
        combatState = CombatState.PlayerTurn;
        
        AddEnergy(energyPerTurn);
        
        if (actionButtonsPanel != null) 
        {
            actionButtonsPanel.SetActive(true);
            UpdateButtonInteractivity();
        }
    }
    
    private void UpdateButtonInteractivity()
    {
        // Enable/disable buttons based on energy
        if (attackButton != null)
            attackButton.interactable = currentEnergy >= attackEnergyCost;
            
        if (defenseButton != null)
            defenseButton.interactable = currentEnergy >= defenseEnergyCost;
            
        if (abilityButton != null)
            abilityButton.interactable = currentEnergy >= abilityCost;
    }
    
    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        
        if (currentEnemy != null)
        {
            currentEnemy.TakeTurn();
            
            // If the enemy has special ability and decides to use it
            if (currentEnemy.enemyData != null && 
                currentEnemy.enemyData.hasSpecialAbility && 
                Random.value < currentEnemy.enemyData.specialAbilityChance)
            {
                // Apply special ability damage
                playerUnit.TakeDamage(currentEnemy.enemyData.specialAbilityDamage);
            }
            else
            {
                // Regular attack
                playerUnit.TakeDamage(enemyUnit.baseDamage);
            }
        }
        else
        {
            // Fallback to basic attack if not using Enemy class
            playerUnit.TakeDamage(enemyUnit.baseDamage);
        }
        
        playerHUD.SetHealth(playerUnit.currentHealth);

        yield return new WaitForSeconds(1f);
        
        if (combatState != CombatState.Lost)
        {
            StartCoroutine(StartPlayerTurn());
        }
    }
    
    // Energy management methods
    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        UpdateEnergyDisplay();
    }
    
    public bool SpendEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            UpdateEnergyDisplay();
            return true;
        }
        return false;
    }
    
    private void UpdateEnergyDisplay()
    {
        if (energyText != null) energyText.text = currentEnergy.ToString() + " / " + maxEnergy.ToString();
        UpdateButtonInteractivity();
    }
    
    // Button action handlers
    public void OnAttackButton()
    {
        if (combatState != CombatState.PlayerTurn || !SpendEnergy(attackEnergyCost)) 
            return;
        
        StartCoroutine(PlayerAttack());
    }
    
    public void OnDefendButton()
    {
        if (combatState != CombatState.PlayerTurn || !SpendEnergy(defenseEnergyCost)) 
            return;
        
        StartCoroutine(PlayerDefend());
    }
    
    public void OnAbilityButton()
    {
        if (combatState != CombatState.PlayerTurn || !SpendEnergy(abilityCost)) 
            return;
        
        StartCoroutine(PlayerAbility());
    }
    
    public void OnSkipButton()
    {
        if (combatState != CombatState.PlayerTurn) 
            return;
        
        // Skip turn gives a bonus energy
        AddEnergy(1);
        
        StartCoroutine(PlayerSkip());
    }
    
    private IEnumerator ExecutePlayerAction(System.Action action)
    {
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        
        action?.Invoke();
        
        yield return new WaitForSeconds(1f);
        
        if (combatState == CombatState.PlayerTurn)
        {
            combatState = CombatState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }


    private IEnumerator PlayerAttack()
    {
        yield return ExecutePlayerAction(() =>
        {
            enemyUnit.TakeDamage(playerUnit.baseDamage);
            enemyHUD.SetHealth(enemyUnit.currentHealth);
        });
    }
    
    private IEnumerator PlayerDefend()
    {
        yield return ExecutePlayerAction(() =>
        {
            playerUnit.SetDefenseMultiplier(defendMultiplier);
        });
    }
    
    private IEnumerator PlayerAbility()
    {
        yield return ExecutePlayerAction(() =>
        {
            enemyUnit.TakeDamage(playerUnit.baseDamage * abilityDamageMultiplier);
            enemyHUD.SetHealth(enemyUnit.currentHealth);
        });
    }
    
    private IEnumerator PlayerSkip()
    {
        yield return ExecutePlayerAction(() =>
        {
            playerUnit.SetDefenseMultiplier(skipMultiplier);
            playerHUD.SetHealth(playerUnit.currentHealth);
        });
    }
    
    private void EndCombat()
    {
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false); 
        StartCoroutine(EndCombatSequence());
    }
    
    private IEnumerator EndCombatSequence()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (GameManager.Instance != null && playerUnit != null)
        {
            float healthPercentage = (float)playerUnit.currentHealth / playerUnit.maxHealth;
            int playerHealthValue = Mathf.RoundToInt(healthPercentage * GameManager.Instance.PlayerData.maxHealth);
            
            GameManager.Instance.PlayerData.currentHealth = playerHealthValue;

            // Fire health changed event to update any UI
            GameManager.Instance.PlayerData.onHealthChanged?.Invoke(playerHealthValue);
            
            GameManager.Instance.SaveGame();
        }
        
        // Hide individual characters if they exist
        if (playerUnit != null && playerUnit.gameObject != null) playerUnit.gameObject.SetActive(false);
        
        if (enemyUnit != null && enemyUnit.gameObject != null) enemyUnit.gameObject.SetActive(false);
        
        // Hide battle stations if using a combat area parent
        if (combatArea != null)
            combatArea.SetActive(false);
        
        // Hide HUD elements
        if (playerHUD != null && playerHUD.gameObject != null)
            playerHUD.gameObject.SetActive(false);
            
        if (enemyHUD != null && enemyHUD.gameObject != null)
            enemyHUD.gameObject.SetActive(false);
        
        // Check if enemy is a boss
        bool isBossFight = currentEnemy != null && currentEnemy is BossEnemy;
        
        // Show appropriate end game panel
        if (combatState == CombatState.Won)
        {
            // For boss fight, show victory panel
            if (isBossFight && victoryPanel != null)
            {
                // Show the victory panel
                victoryPanel.SetActive(true);
            }
            // For regular enemies, show rewards
            else
            {
                if (rewardSystem != null)
                {
                    rewardSystem.ShowRewards();
                }
            }
        }
        else if (combatState == CombatState.Lost)
        {
            if (gameOverPanel != null)
            {
                GameManager.Instance?.PlayerData?.HandleDefeat();
                GameManager.Instance?.SaveGame();
                gameOverPanel.SetActive(true);
            }
        }
    }
    
    public void ReturnToMap()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.Map);
        }
    }
    
    public void RestartCombat()
    {
        // Hide end game panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        
        // Also hide reward panel if it exists
        if (rewardSystem != null && rewardSystem.rewardPanel != null) 
            rewardSystem.rewardPanel.SetActive(false);
        
        // Show HUD elements
        if (playerHUD != null && playerHUD.gameObject != null)
            playerHUD.gameObject.SetActive(true);
            
        if (enemyHUD != null && enemyHUD.gameObject != null)
            enemyHUD.gameObject.SetActive(true);
            
        // Clean up existing characters
        if (playerUnit != null) Destroy(playerUnit.gameObject);
        if (enemyUnit != null) Destroy(enemyUnit.gameObject);
        
        Start();
    }
} 