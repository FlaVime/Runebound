using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public enum CombatState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    SelectingTarget,
    Won,
    Lost
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Setup")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    [Header("Units")]
    CharacterBase playerUnit;
    CharacterBase enemyUnit;

    [Header("Energy Management")]
    public int maxEnergy = 3;
    public int currentEnergy = 3;
    public int attackEnergyCost = 1;
    public int defenseEnergyCost = 1;
    public int abilityCost = 2;
    public int energyPerTurn = 1;
    
    [Header("UI")]
    public CombatHUD playerHUD;
    public CombatHUD enemyHUD;
    public GameObject actionButtonsPanel;
    public GameObject gameOverPanel;
    public GameObject rewardPanel;
    public TMP_Text energyText;
    public Button attackButton;
    public Button defenseButton;
    public Button abilityButton;
    public GameObject combatArea; // Parent object containing battle stations and characters
    
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
        
        // Find RewardSystem if not assigned
        if (rewardSystem == null)
        {
            rewardSystem = FindObjectOfType<RewardSystem>();
            if (rewardSystem == null)
            {
                Debug.LogError("No RewardSystem found in the scene. Rewards won't work!");
            }
        }
        
        // Hide panels initially
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }
    
    void Start()
    {
        combatState = CombatState.Start;
        SetupCombat();
    }

    public void SetupCombat()
    {
        // Show combat area if it exists
        if (combatArea != null)
        {
            combatArea.SetActive(true);
        }
        
        // Reset energy
        currentEnergy = maxEnergy;
        UpdateEnergyDisplay();
        
        // Spawn player character
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<CharacterBase>();

        // Spawn enemy character
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<CharacterBase>();

        // Setup UI
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        
        // Subscribe to health change events
        playerUnit.onHealthChanged.AddListener(OnPlayerHealthChanged);
        enemyUnit.onHealthChanged.AddListener(OnEnemyHealthChanged);
        
        // Start player turn
        StartCoroutine(StartPlayerTurn());
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
        
        // Add energy at the start of each turn
        AddEnergy(energyPerTurn);
        
        // Enable action buttons panel
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
        
        // Simple AI - enemy always attacks
        if (enemyUnit.currentHealth > 0)
        {
            playerUnit.TakeDamage(enemyUnit.baseDamage);
            
            playerHUD.SetHealth(playerUnit.currentHealth);

            yield return new WaitForSeconds(1f);
            
            if (combatState != CombatState.Lost)
            {
                StartCoroutine(StartPlayerTurn());
            }
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
        if (energyText != null)
        {
            energyText.text = currentEnergy.ToString() + " / " + maxEnergy.ToString();
        }
        
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
    
    private IEnumerator PlayerAttack()
    {
        // Disable action buttons during attack
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        
        enemyUnit.TakeDamage(playerUnit.baseDamage);
        
        enemyHUD.SetHealth(enemyUnit.currentHealth);

        yield return new WaitForSeconds(1f);
        
        if (combatState == CombatState.PlayerTurn)
        {
            // Switch to enemy turn
            combatState = CombatState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    
    private IEnumerator PlayerDefend()
    {
        // Disable action buttons during defend
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        
        playerUnit.SetDefenseMultiplier(0.5f); // Take half damage
        
        yield return new WaitForSeconds(1f);
        
        if (combatState == CombatState.PlayerTurn)
        {
            // Switch to enemy turn
            combatState = CombatState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    
    private IEnumerator PlayerAbility()
    {
        // Disable action buttons during ability
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        
        enemyUnit.TakeDamage(playerUnit.baseDamage * 1.5f);
        
        enemyHUD.SetHealth(enemyUnit.currentHealth);

        yield return new WaitForSeconds(1f);
        
        if (combatState == CombatState.PlayerTurn)
        {
            // Switch to enemy turn
            combatState = CombatState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    
    private IEnumerator PlayerSkip()
    {
        // Disable action buttons during skip
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        
        playerUnit.SetDefenseMultiplier(1.5f); // Take more damage, but recover
        
        playerHUD.SetHealth(playerUnit.currentHealth);

        yield return new WaitForSeconds(1f);
        
        if (combatState == CombatState.PlayerTurn)
        {
            // Switch to enemy turn
            combatState = CombatState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    
    private void EndCombat()
    {
        // Disable action buttons
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        
        // Hide combat area to hide characters and battle stations
        StartCoroutine(EndCombatSequence());
    }
    
    private IEnumerator EndCombatSequence()
    {
        // Short delay before hiding characters
        yield return new WaitForSeconds(0.5f);
        
        // Hide individual characters if they exist
        if (playerUnit != null && playerUnit.gameObject != null)
        {
            playerUnit.gameObject.SetActive(false);
        }
        
        if (enemyUnit != null && enemyUnit.gameObject != null)
        {
            enemyUnit.gameObject.SetActive(false);
        }
        
        // Hide battle stations if using a combat area parent
        if (combatArea != null)
        {
            combatArea.SetActive(false);
        }
        else
        {
            // If no combat area exists, try to hide the battle stations directly
            if (playerBattleStation != null && playerBattleStation.gameObject != null)
                playerBattleStation.gameObject.SetActive(false);
                
            if (enemyBattleStation != null && enemyBattleStation.gameObject != null)
                enemyBattleStation.gameObject.SetActive(false);
        }
        
        // Hide HUD elements
        if (playerHUD != null && playerHUD.gameObject != null)
            playerHUD.gameObject.SetActive(false);
            
        if (enemyHUD != null && enemyHUD.gameObject != null)
            enemyHUD.gameObject.SetActive(false);
        
        // Show appropriate end game panel
        if (combatState == CombatState.Won)
        {
            // Add a debug log to check if this part is reached
            Debug.Log("Victory! Showing rewards");
            
            // Show victory panel if no reward system exists
            if (rewardSystem != null)
            {
                // Use reward system to show reward choices
                rewardSystem.ShowRewards();
                
                // Add another debug to see if ShowRewards is called
                Debug.Log("Called ShowRewards on RewardSystem");
            }
            else if (rewardPanel != null) 
            {
                rewardPanel.SetActive(true);
                
                // Add a debug log
                Debug.Log("No RewardSystem found, showing basic rewardPanel");
                
                // Give rewards directly if no reward system
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GiveRewards(10, 5, 20);
                }
            }
            else
            {
                Debug.LogError("No reward system or reward panel found! Player won't see any victory UI.");
            }
        }
        else if (combatState == CombatState.Lost)
        {
            if (gameOverPanel != null) 
                gameOverPanel.SetActive(true);
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
        
        // Also hide reward panel if it exists
        if (rewardSystem != null && rewardSystem.rewardPanel != null)
        {
            rewardSystem.rewardPanel.SetActive(false);
        }
        
        // Show HUD elements
        if (playerHUD != null && playerHUD.gameObject != null)
            playerHUD.gameObject.SetActive(true);
            
        if (enemyHUD != null && enemyHUD.gameObject != null)
            enemyHUD.gameObject.SetActive(true);
            
        // Clean up existing characters
        if (playerUnit != null) Destroy(playerUnit.gameObject);
        if (enemyUnit != null) Destroy(enemyUnit.gameObject);
        
        // Start new combat
        Start();
    }
} 