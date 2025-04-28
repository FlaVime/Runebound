using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : Singleton<SceneLoader> {
    public void Load(string sceneName) {
        StartCoroutine(DoLoad(sceneName));
    }

    private IEnumerator DoLoad(string sceneName) {
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
    }
}
