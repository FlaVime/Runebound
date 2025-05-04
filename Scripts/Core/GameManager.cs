using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState {
    MainMenu,
    Map,
    Combat,
    Event,
    Shop,
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
        
        LoadGame();
        
        if (playerData.currentHealth <= 0)
        {
            playerData.Init();
        }
        
        playerData.onHealthChanged.AddListener(CheckPlayerHealth);
    }

    private void Start() {
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            ChangeState(GameState.MainMenu);
        } else {
            LoadGame();
        }
    }
    
    private void OnEnable()
    {
        SceneLoader.OnSceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneLoader.OnSceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(string sceneName)
    {
        SaveGame();
    }

    public void ChangeState(GameState newState) {
        if (newState == CurrentState)
            return;
            
        SaveGame();
            
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        
        SceneLoader.Instance.Load(newState.ToString());
    }
    
    private void CheckPlayerHealth(int health)
    {
        if (health <= 0 && CurrentState != GameState.GameOver)
        {
            ChangeState(GameState.GameOver);
        }
    }
    
    public void GiveRewards(int gold, int souls)
    {
        playerData.AddGold(gold);
        playerData.AddSouls(souls);
        
        SaveGame();
        
        MapSystem mapSystem = FindFirstObjectByType<MapSystem>();
        if (mapSystem != null)
        {
            mapSystem.SaveMapState();
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }
    
    public void SaveGame()
    {
        MapSystem mapSystem = FindFirstObjectByType<MapSystem>();
        if (mapSystem != null)
        {
            mapSystem.SaveMapState();
        }
        
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("SavedGame", json);
        PlayerPrefs.Save();
    }
    
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {            
            string json = PlayerPrefs.GetString("SavedGame");
            JsonUtility.FromJsonOverwrite(json, playerData);            
        }
    }
    
    public void ResetGame()
    {
        PlayerPrefs.DeleteKey("SavedGame");
        
        PlayerPrefs.DeleteKey("CurrentNodeIndex");
        PlayerPrefs.DeleteKey("VisitedNodeIndices");
        
        playerData.Init();
    }

    public void StartNewGame() 
    {
        PlayerPrefs.DeleteKey("SavedGame");
        PlayerPrefs.DeleteKey("MapSaveData");
        PlayerPrefs.DeleteKey("VisitedNodeIndices");
        PlayerPrefs.Save();
        
        playerData = new PlayerData();
        playerData.Init();

        SaveGame();

        SceneManager.LoadScene("Map");
    }

    public void ContinueGame() 
    {
        LoadGame();
        SceneManager.LoadScene("Map");
    }

    public void RestartAfterDefeat() 
    {
        PlayerPrefs.DeleteKey("MapSaveData");
        PlayerPrefs.DeleteKey("CurrentNodeIndex");
        PlayerPrefs.DeleteKey("VisitedNodeIndices");
        PlayerPrefs.Save();

        SaveGame();
        SceneManager.LoadScene("Map");
    }

}
