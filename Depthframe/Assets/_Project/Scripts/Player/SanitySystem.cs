// Core Sanity System Design (C# - Unity)

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity;
    public float sanityDrainRate = 5f; // per second in darkness
    public float sanityRestoreRate = 3f; // per second in light
    
    [Header("References")]
    public Light2D playerLight;
    
    public bool IsInLight => playerLight.intensity > 0.1f;
    
    public delegate void OnSanityChanged(float sanity);
    public static event OnSanityChanged SanityChanged;

    private void Start()
    {
        currentSanity = maxSanity;
    }

    private void Update()
    {
        float delta = Time.deltaTime * (IsInLight ? sanityRestoreRate : -sanityDrainRate);
        currentSanity = Mathf.Clamp(currentSanity + delta, 0, maxSanity);
        
        SanityChanged?.Invoke(currentSanity);
        
        HandleSanityEffects();
    }

    void HandleSanityEffects()
    {
        if (currentSanity < 25)
        {
            TraumaManager.Instance.TriggerCriticalEffects();
        }
        else if (currentSanity < 50)
        {
            TraumaManager.Instance.TriggerMediumEffects();
        }
        else if (currentSanity < 75)
        {
            TraumaManager.Instance.TriggerMildEffects();
        }
        else
        {
            TraumaManager.Instance.ClearEffects();
        }
    }
}

public class TraumaManager : MonoBehaviour
{
    public static TraumaManager Instance;
    
    [Header("Effect Objects")]
    public AudioSource whisperSource;
    public GameObject ghostPrefab;
    public Transform ghostSpawnPoint;
    public UnityEngine.Rendering.Volume postFXVolume;
    public Light2D flashlight;
    public float flickerIntensityMin = 0.5f;
    public float flickerIntensityMax = 1.2f;
    public float flickerSpeed = 0.05f;
    
    private UnityEngine.Rendering.Universal.Vignette vignette;
    private bool isFlickering = false;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (postFXVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.Override(0f);
        }
    }
    
    public void TriggerMildEffects()
    {
        vignette.intensity.value = 0.2f;
    }

    public void TriggerMediumEffects()
    {
        vignette.intensity.value = 0.4f;
        if (!whisperSource.isPlaying) whisperSource.Play();
        if (!isFlickering) StartCoroutine(FlickerFlashlight());
    }

    public void TriggerCriticalEffects()
    {
        vignette.intensity.value = 0.6f;
        if (!whisperSource.isPlaying) whisperSource.Play();
        if (ghostPrefab != null)
        {
            Instantiate(ghostPrefab, ghostSpawnPoint.position, Quaternion.identity);
        }
        if (!isFlickering) StartCoroutine(FlickerFlashlight());
    }

    public void ClearEffects()
    {
        vignette.intensity.value = 0f;
        if (whisperSource.isPlaying) whisperSource.Stop();
        StopAllCoroutines();
        if (flashlight != null) flashlight.intensity = 1f;
        isFlickering = false;
    }

    private System.Collections.IEnumerator FlickerFlashlight()
    {
        isFlickering = true;
        while (true)
        {
            if (flashlight != null)
            {
                flashlight.intensity = Random.Range(flickerIntensityMin, flickerIntensityMax);
            }
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}





