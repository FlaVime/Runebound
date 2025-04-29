using UnityEngine;

public enum GameState {
    MainMenu,
    Map,
    Combat,
    Event,
    Shop,
    Rest,
    Boss
}

public class GameManager : Singleton<GameManager> {
    public GameState CurrentState { get; private set; }

    public int playerHP {get; private set; } = 100;
    public int playerGold {get; private set; } = 0;
    public int playerSouls {get; private set; } = 0;

    protected override void Awake() {
        base.Awake(); // Вызываем базовый Awake из Singleton
        DontDestroyOnLoad(gameObject); // Делаем объект неуничтожимым при смене сцены
    }

    private void Start() {
        ChangeState(GameState.Map); // CHANGE HERE TO MAIN MENU OR MAP
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        SceneLoader.Instance.Load(newState.ToString());
    }

    // Player stats management
    public void AddGold(int amount) {
        playerGold += amount;
        // Можно вызвать обновление UI
    }

    public void AddSouls(int amount) {
        playerSouls += amount;
    }

    public void TakeDamage(int damage) {
        playerHP = Mathf.Max(0, playerHP - damage);
    }
}
