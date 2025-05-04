using UnityEngine.SceneManagement;
using System;

public class SceneLoader : Singleton<SceneLoader> {
    public static event Action<string> OnSceneLoaded;
    
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoadComplete;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadComplete;
    }
    
    private void OnSceneLoadComplete(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoaded?.Invoke(scene.name);
    }
    
    public void Load(string sceneName) {
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
            if (GameManager.Instance.CurrentState != GameState.MainMenu && SceneManager.GetSceneByName("MainMenu") != null)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
