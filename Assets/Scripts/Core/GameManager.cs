using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState {
    MainMenu,
    Map,
    Combat,
    Event,
    Shop,
    Boss
}

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }

    [SerializeField] private PlayerData playerData = new PlayerData();
    public PlayerData PlayerData => playerData;

    private void InitPlayerData()
    {
        if (!PlayerPrefs.HasKey("SavedGame"))
        {
            playerData.Init();
        }
        else
        {
            LoadGame();
        }

        if (playerData.currentHealth.Value <= 0)
        {
            playerData.Init();
        }
    }


    public event Action<GameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey("SavedGame"))
        {
            playerData.Init();
        }
        else
        {
            LoadGame();
        }

        if (playerData.currentHealth.Value <= 0)
        {
            playerData.Init();
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ChangeState(GameState.MainMenu);
        }
        else
        {
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

    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState)
            return;

        SaveGame();

        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        if (MusicManager.Instance != null)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    MusicManager.Instance.PlayMenuMusic();
                    break;
                case GameState.Map:
                case GameState.Shop:
                case GameState.Event:
                    MusicManager.Instance.PlayMapMusic();
                    break;
                case GameState.Combat:
                    MusicManager.Instance.PlayRandomCombatMusic();
                    break;
                case GameState.Boss:
                    MusicManager.Instance.PlayRandomBossMusic();
                    break;
            }
        }

        SceneLoader.Instance.Load(newState.ToString());
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
        BackupSave();

        DatabaseManager.Instance.SavePlayerUpgrade(playerData.purchasedUpgrades);
    }

    public void BackupSave()
    {
        string json = PlayerPrefs.GetString("SavedGame", null);
        if (!string.IsNullOrEmpty(json))
        {
            string backupPath = System.IO.Path.Combine(Application.persistentDataPath, "BackupSavedGame.json");
            System.IO.File.WriteAllText(backupPath, json);
            Debug.Log("Game backup created at: " + backupPath);
            GameLogger.Log("Game backup created");
        }
        else
        {
            Debug.LogWarning("No game data to backup.");
            GameLogger.Log("No game data to backup");
        }
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            string json = PlayerPrefs.GetString("SavedGame");
            JsonUtility.FromJsonOverwrite(json, playerData);

            playerData.purchasedUpgrades = DatabaseManager.Instance.LoadPlayerUpgrade();
        }
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
        ChangeState(GameState.Map);
    }

    public void ContinueGame()
    {
        LoadGame();

        // Check if the player is dead
        if (playerData.currentHealth.Value <= 0)
        {
            playerData.currentHealth.Set(playerData.maxHealth);
            playerData.onHealthChanged?.Invoke(playerData.maxHealth);
        }

        ChangeState(GameState.Map);
    }

    public void ApplyEventOutcome(int gold, int souls, int health)
    {
        if (gold != 0)
            playerData.AddGold(gold);

        if (souls != 0)
            playerData.AddSouls(souls);

        if (health > 0)
            playerData.Heal(health);
        else if (health < 0)
            playerData.TakeDamage(-health);

        SaveGame();
    }
    
    public void ResetMapOnly()
    {
        PlayerPrefs.DeleteKey("MapSaveData");
        PlayerPrefs.DeleteKey("VisitedNodeIndices");
        PlayerPrefs.DeleteKey("CurrentNodeIndex");
        PlayerPrefs.Save();
    }
}
