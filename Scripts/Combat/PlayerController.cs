// using UnityEngine;
// using UnityEngine.Events;

// public class PlayerController : CharacterBase
// {
//     [Header("References")]
//     private PlayerCharacter playerCharacter;
    
//     [Header("Energy")]
//     private int maxEnergy;
//     private int currentEnergy;
    
//     public UnityEvent<int> onEnergyChanged;
//     public UnityEvent<Enemy> onEnemySelected;

//     private Enemy selectedTarget;
//     private ActionType selectedAction;

//     protected override void Start()
//     {
//         base.Start();
        
//         // Get player character reference
//         playerCharacter = GetComponent<PlayerCharacter>();
//         if (playerCharacter == null)
//         {
//             playerCharacter = gameObject.AddComponent<PlayerCharacter>();
//         }
        
//         // Get data from GameManager
//         maxEnergy = GameManager.Instance.PlayerData.maxEnergy;
//         currentEnergy = GameManager.Instance.PlayerData.energy;
//         maxHealth = GameManager.Instance.PlayerData.maxHealth;
//         currentHealth = GameManager.Instance.PlayerData.currentHealth;
//         baseDamage = GameManager.Instance.PlayerData.baseDamage;
        
//         // Update UI
//         UpdateEnergyUI();
//     }

//     public void SelectAction(ActionType action)
//     {
//         selectedAction = action;
        
//         switch (action)
//         {
//             case ActionType.Attack:
//                 if (currentEnergy >= 1)
//                 {
//                     CombatManager.Instance.SetState(CombatState.SelectingTarget);
//                 }
//                 break;
                
//             case ActionType.Defence:
//                 if (currentEnergy >= 1)
//                 {
//                     UseDefense();
//                     EndTurn();
//                 }
//                 break;
                
//             case ActionType.Ability:
//                 if (currentEnergy >= 2)
//                 {
//                     CombatManager.Instance.SetState(CombatState.SelectingTarget);
//                 }
//                 break;
                
//             case ActionType.Skip:
//                 SkipTurn();
//                 EndTurn();
//                 break;
//         }
//     }

//     public void SelectTarget(Enemy target)
//     {
//         if (CombatManager.Instance.CurrentState != CombatState.SelectingTarget)
//             return;

//         selectedTarget = target;
//         onEnemySelected?.Invoke(target);
//         ExecuteAction();
//     }

//     private void ExecuteAction()
//     {
//         switch (selectedAction)
//         {
//             case ActionType.Attack:
//                 if (currentEnergy >= 1)
//                 {
//                     selectedTarget.TakeDamage(baseDamage);
//                     SpendEnergy(1);
//                 }
//                 break;
                
//             case ActionType.Ability:
//                 if (currentEnergy >= 2)
//                 {
//                     selectedTarget.TakeDamage(baseDamage * 1.5f);
//                     SpendEnergy(2);
//                 }
//                 break;
//         }
        
//         EndTurn();
//     }

//     private void UseDefense()
//     {
//         SetDefenseMultiplier(0.5f);
//         SpendEnergy(1);
//     }

//     private void SkipTurn()
//     {
//         SetDefenseMultiplier(1.5f);
//         AddEnergy(2);
//     }

//     private void SpendEnergy(int amount)
//     {
//         GameManager.Instance.PlayerData.SpendEnergy(amount);
//         currentEnergy = GameManager.Instance.PlayerData.energy;
//         UpdateEnergyUI();
//     }
    
//     private void AddEnergy(int amount)
//     {
//         GameManager.Instance.PlayerData.AddEnergy(amount);
//         currentEnergy = GameManager.Instance.PlayerData.energy;
//         UpdateEnergyUI();
//     }

//     private void UpdateEnergyUI()
//     {
//         onEnergyChanged?.Invoke(currentEnergy);
//     }

//     private void EndTurn()
//     {
//         selectedTarget = null;
//         CombatManager.Instance.SetState(CombatState.EnemyTurn);
//     }

//     public override void TakeDamage(float damage)
//     {
//         // Apply damage to player
//         float actualDamage = damage * defenseMultiplier;
//         GameManager.Instance.PlayerData.TakeDamage((int)actualDamage);
        
//         // Update current health from PlayerData
//         currentHealth = GameManager.Instance.PlayerData.currentHealth;
        
//         // Send event for UI updates
//         onHealthChanged?.Invoke(currentHealth / maxHealth);
        
//         // Reset defense modifier after taking damage
//         SetDefenseMultiplier(1f);
//     }
// } 