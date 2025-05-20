using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip mapMusic;
    [SerializeField] private AudioMixer audioMixer;

    private string currentTrack = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayMenuMusic();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMenuMusic();
                break;
            case "Map":
                PlayMapMusic();
                break;
        }
    }

    public void PlayMenuMusic()
    {
        PlayTrack(mainMenuMusic, "MainMenu");
    }

    public void PlayMapMusic()
    {
        PlayTrack(mapMusic, "Map");
    }

    private void PlayTrack(AudioClip clip, string trackName)
    {
        if (clip == null || musicSource == null || currentTrack == trackName)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        currentTrack = trackName;
    }
}
