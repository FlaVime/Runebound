using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneLoader : Singleton<SceneLoader>
{
    public static event Action<string> OnSceneLoaded;

    [Header("Transition Animation")]
    public Animator transition;
    public float transitionTime = 2f;

    protected override void Awake()
    {
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

    public void Load(string sceneName)
    {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
        }

        SceneManager.LoadScene(sceneName);
    }
}