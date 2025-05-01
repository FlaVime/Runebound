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

    [Header("UI")]
    public CombatHUD playerHUD;
    public CombatHUD enemyHUD;
    public GameObject actionButtonsPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

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
        
        // Hide panels initially
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }
    
    void Start()
    {
        combatState = CombatState.Start;
        SetupCombat();
    }

    public void SetupCombat()
    {
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
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(true);
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
    
    // Button action handlers
    public void OnAttackButton()
    {
        if (combatState != CombatState.PlayerTurn) return;
        
        StartCoroutine(PlayerAttack());
    }
    
    public void OnDefendButton()
    {
        if (combatState != CombatState.PlayerTurn) return;
        
        StartCoroutine(PlayerDefend());
    }
    
    public void OnAbilityButton()
    {
        if (combatState != CombatState.PlayerTurn) return;
        
        StartCoroutine(PlayerAbility());
    }
    
    public void OnSkipButton()
    {
        if (combatState != CombatState.PlayerTurn) return;
        
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
        
        if (combatState == CombatState.Won)
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
            
            // Give rewards
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GiveRewards(10, 5, 20);
            }
        }
        else if (combatState == CombatState.Lost)
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
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
        // Clean up
        if (playerUnit != null) Destroy(playerUnit.gameObject);
        if (enemyUnit != null) Destroy(enemyUnit.gameObject);
        
        // Reset UI
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        
        // Start new combat
        Start();
    }
} 