public class Boss : CharacterBase {
    public int specialAbilityPower; // Сила специальной способности

    public void UseSpecialAbility(CharacterBase target) {
        target.TakeDamage(specialAbilityPower); // Пример: босс наносит доп. урон
    }
}