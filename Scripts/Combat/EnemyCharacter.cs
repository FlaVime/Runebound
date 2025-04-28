using UnityEngine;

public class EnemyCharacter : CharacterBase {
    public void Attack(PlayerCharacter target) {
        Debug.Log($"Enemy attacks Player for 2 damage");
        target.TakeDamage(2);
    }

    protected override void Die() {
        base.Die();
        Debug.Log("=== Victory! Enemy defeated ===");
        // Здесь можно сделать Reward, переход дальше и т.д.
    }
}