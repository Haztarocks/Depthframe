using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        [HideInInspector]
        public AudioSource source;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip sanityLowMusic;
    [SerializeField] private float musicFadeTime = 2f;
    [SerializeField] private float lowSanityThreshold = 30f;

    [Header("Sound Effects")]
    public SoundEffect[] soundEffects;

    private Dictionary<string, SoundEffect> soundDictionary = new Dictionary<string, SoundEffect>();
    private float originalMusicVolume;
    private AudioClip currentMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to scene load events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Subscribe to sanity change events
        SanitySystem.SanityChanged += OnSanityChanged;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        SanitySystem.SanityChanged -= OnSanityChanged;
    }

    private void InitializeAudio()
    {
        // Initialize music source if not set
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.parent = transform;
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        // Initialize SFX source if not set
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX Source");
            sfxObj.transform.parent = transform;
            sfxSource = sfxObj.AddComponent<AudioSource>();
        }

        // Store original music volume
        originalMusicVolume = musicSource.volume;

        // Create dictionary of sound effects
        foreach (SoundEffect sound in soundEffects)
        {
            soundDictionary[sound.name] = sound;
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Play appropriate music for the scene
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "Intro":
            case "Game":
                PlayMusic(gameplayMusic);
                break;
        }
    }

    private void OnSanityChanged(float sanity)
    {
        if (sanity <= lowSanityThreshold)
        {
            if (currentMusic != sanityLowMusic)
            {
                PlayMusic(sanityLowMusic);
            }
        }
        else if (currentMusic == sanityLowMusic)
        {
            // Return to normal gameplay music when sanity recovers
            PlayMusic(gameplayMusic);
        }
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == null || music == currentMusic) return;

        currentMusic = music;
        StartCoroutine(FadeMusicRoutine(music));
    }

    private System.Collections.IEnumerator FadeMusicRoutine(AudioClip newMusic)
    {
        float timeElapsed = 0;
        float startVolume = musicSource.volume;

        // Fade out current music
        while (timeElapsed < musicFadeTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / musicFadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Change to new music
        musicSource.clip = newMusic;
        musicSource.Play();

        // Fade in new music
        timeElapsed = 0;
        while (timeElapsed < musicFadeTime)
        {
            musicSource.volume = Mathf.Lerp(0, originalMusicVolume, timeElapsed / musicFadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = originalMusicVolume;
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundEffect sound))
        {
            sfxSource.pitch = sound.pitch;
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }
}