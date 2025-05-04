using UnityEngine;

public class BossEnemy : Enemy
{
    public bool enraged = false;

    public new void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (!enraged && currentHealth / maxHealth < 0.4f)
        {
            enraged = true;
            baseDamage *= 1.5f;
        }
    }
}