using UnityEngine;
using System.Collections;

public class CombatManager : Singleton<CombatManager> {
    [Header("Prefabs & Spawns")]
    public PlayerCharacter playerPrefab;
    public EnemyCharacter enemyPrefab;
    public Transform playerSpawn;
    public Transform enemySpawn;

    private PlayerCharacter player;
    private EnemyCharacter enemy;
    private bool playerTurn = true;

    private void Start() {
        StartCoroutine(BeginCombat());
    }

    private IEnumerator BeginCombat() {
        Debug.Log("=== Combat Started ===");
        player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        enemy  = Instantiate(enemyPrefab , enemySpawn .position, Quaternion.identity);

        // Небольшая задержка перед первым ходом
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Player's turn");
    }

    private void Update() {
        // Для теста: нажми пробел, чтобы атаковать
        if (playerTurn && Input.GetKeyDown(KeyCode.Space)) {
            PlayerAttack();
        }
    }

    private void PlayerAttack() {
        if (enemy == null) return;
        player.Attack(enemy);
        playerTurn = false;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn() {
        yield return new WaitForSeconds(1f);
        if (enemy != null && player != null) {
            enemy.Attack(player);
        }
        if (player != null) {
            Debug.Log("Player's turn");
            playerTurn = true;
        }
    }
}
