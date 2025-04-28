using UnityEngine;

public class CharacterBase : MonoBehaviour {
    public int maxHP;
    public int currentHP;
    public int attackPower;

    public virtual void TakeDamage(int damage) {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
    }

    public virtual void Attack(CharacterBase target) {
        target.TakeDamage(attackPower);
    }
}