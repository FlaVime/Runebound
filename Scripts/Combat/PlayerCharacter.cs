using UnityEngine;

public class PlayerCharacter : CharacterBase {
    public void Attack(EnemyCharacter target) {
        Debug.Log($"Player attacks Enemy for 3 damage");
        target.TakeDamage(3);
    }

    protected override void Die() {
        base.Die();
        Debug.Log("=== Game Over ===");
        // Здесь можно сделать Reset или переход в меню
    }
}