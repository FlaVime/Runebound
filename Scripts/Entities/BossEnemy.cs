using UnityEngine;

public class BossEnemy : Enemy
{
    public bool enraged = false;

    public new void TakeTurn()
    {
        // check if boss is in second phase (example: < 40%)
        if (!enraged && currentHealth / maxHealth < 0.4f)
        {
            enraged = true;
            baseDamage *= 1.5f;
            Debug.Log($"{unitName} entered second phase");
        }

        base.TakeTurn();
    }
}