using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Setup")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip mapMusic;
    [SerializeField] private List<AudioClip> combatClips;
    [SerializeField] private List<AudioClip> bossClips;

    private string currentTrack = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneLoader.OnSceneLoaded += HandleSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HandleSceneLoaded(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMenuMusic();
                break;
            case "Map":
                PlayMapMusic();
                break;
            case "Combat":
                PlayRandomCombatMusic();
                break;
            case "Boss":
                PlayRandomBossMusic();
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

    public void PlayRandomCombatMusic()
    {
        if (combatClips.Count == 0) return;
        AudioClip random = combatClips[Random.Range(0, combatClips.Count)];
        PlayTrack(random, "Combat");
    }

    public void PlayRandomBossMusic()
    {
        if (bossClips.Count == 0) return;
        AudioClip random = bossClips[Random.Range(0, bossClips.Count)];
        PlayTrack(random, "Boss");
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
