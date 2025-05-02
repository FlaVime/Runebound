using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : Singleton<SceneLoader> {
    // Event to notify when a scene is loaded
    public static event Action<string> OnSceneLoaded;
    
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        // Register to scene loading events
        SceneManager.sceneLoaded += OnSceneLoadComplete;
    }
    
    private void OnDestroy()
    {
        // Unregister from scene loading events
        SceneManager.sceneLoaded -= OnSceneLoadComplete;
    }
    
    private void OnSceneLoadComplete(Scene scene, LoadSceneMode mode)
    {
        // Notify subscribers that the scene has loaded
        OnSceneLoaded?.Invoke(scene.name);
    }
    
    public void Load(string sceneName) {
        // Check if the scene exists in the build settings
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameFromPath == sceneName)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (sceneExists)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " does not exist in build settings!");
            
            // Load a default scene if the requested one doesn't exist
            if (GameManager.Instance.CurrentState != GameState.MainMenu && SceneManager.GetSceneByName("MainMenu") != null)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
