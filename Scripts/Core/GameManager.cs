using UnityEngine;
using System;

public enum GameState {
    MainMenu,
    Map,
    Combat,
    Event,
    Shop,
    Rest,
    Boss,
    GameOver,
    Victory
}

public class GameManager : Singleton<GameManager> {
    public GameState CurrentState { get; private set; }
    
    [SerializeField] private PlayerData playerData = new PlayerData();
    public PlayerData PlayerData => playerData;
    
    public event Action<GameState> OnGameStateChanged;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        // Initialize player data
        playerData.Init();
        
        // Register event listeners to handle state transitions
        playerData.onHealthChanged.AddListener(CheckPlayerHealth);
    }

    private void Start() {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState) {
        if (newState == CurrentState)
            return;
            
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        
        // Load appropriate scene based on state
        SceneLoader.Instance.Load(newState.ToString());
    }
    
    private void CheckPlayerHealth(int health)
    {
        if (health <= 0 && CurrentState != GameState.GameOver)
        {
            ChangeState(GameState.GameOver);
        }
    }
    
    // Combat rewards
    public void GiveRewards(int gold, int souls, int experience)
    {
        playerData.AddGold(gold);
        playerData.AddSouls(souls);
        playerData.AddExperience(experience);
    }
    
    // Save/Load functionality can be added here
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("SavedGame", json);
        PlayerPrefs.Save();
    }
    
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            string json = PlayerPrefs.GetString("SavedGame");
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
    }
}
