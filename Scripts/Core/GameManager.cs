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
        LoadGame(); // Try to load saved game first
        
        // If no save exists, initialize with default values
        if (playerData.currentHealth <= 0)
        {
            playerData.Init();
        }
        
        // Register event listeners to handle state transitions
        playerData.onHealthChanged.AddListener(CheckPlayerHealth);
    }

    private void Start() {
        ChangeState(GameState.MainMenu);
    }
    
    private void OnEnable()
    {
        // Subscribe to scene load events
        SceneLoader.OnSceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from scene load events
        SceneLoader.OnSceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(string sceneName)
    {
        // Save game each time a scene is loaded
        SaveGame();
    }

    public void ChangeState(GameState newState) {
        if (newState == CurrentState)
            return;
            
        // Save the current state before changing
        SaveGame();
            
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
        
        // Save immediately after giving rewards
        SaveGame();
        
        // Save map state if in a map scene
        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.SaveMapState();
            Debug.Log("Map state saved after giving rewards");
        }
    }
    
    private void OnApplicationQuit()
    {
        // Save before quitting
        SaveGame();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Save when game is paused (e.g., when player switches apps on mobile)
        if (pauseStatus)
        {
            SaveGame();
        }
    }
    
    // Save/Load functionality
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("SavedGame", json);
        PlayerPrefs.Save();
        Debug.Log("Game saved. Gold: " + playerData.gold + ", Souls: " + playerData.souls);
        
        // Also save map state if available
        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.SaveMapState();
        }
    }
    
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            string json = PlayerPrefs.GetString("SavedGame");
            JsonUtility.FromJsonOverwrite(json, playerData);
            Debug.Log("Game loaded. Gold: " + playerData.gold + ", Souls: " + playerData.souls);
        }
        else
        {
            Debug.Log("No saved game found, initializing with defaults");
        }
    }
    
    public void ResetGame()
    {
        // Clear player data
        PlayerPrefs.DeleteKey("SavedGame");
        
        // Clear map state
        PlayerPrefs.DeleteKey("CurrentNodeIndex");
        PlayerPrefs.DeleteKey("VisitedNodeIndices");
        
        playerData.Init();
        
        Debug.Log("Game reset. All progress cleared.");
    }
}
