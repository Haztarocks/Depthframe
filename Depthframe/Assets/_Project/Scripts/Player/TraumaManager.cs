using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class TraumaManager : MonoBehaviour
{
    public static TraumaManager Instance { get; private set; }

    [Header("References")]
    public Light2D flashlight;
    public VolumeProfile volumeProfile;
    public AudioSource whisperSource;
    public GameObject ghostPrefab;
    public Transform ghostSpawnPoint;

    [Header("Flicker Settings")]
    public float flickerIntensityMin = 0.1f;
    public float flickerIntensityMax = 1.0f;

    private Vignette vignette;
    private bool isFlickering;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (volumeProfile != null)
        {
            volumeProfile.TryGet(out vignette);
        }
    }

    public void TriggerCriticalEffects()
    {
        if (flashlight != null)
        {
            StartCoroutine(FlickerLight(0.1f));
        }
        
        if (vignette != null)
        {
            vignette.intensity.Override(0.8f);
        }
        
        if (whisperSource != null && !whisperSource.isPlaying)
        {
            whisperSource.Play();
        }
        
        if (ghostPrefab != null && ghostSpawnPoint != null)
        {
            Instantiate(ghostPrefab, ghostSpawnPoint.position, Quaternion.identity);
        }
    }
    
    public void TriggerMediumEffects()
    {
        if (flashlight != null)
        {
            StartCoroutine(FlickerLight(0.2f));
        }
        
        if (vignette != null)
        {
            vignette.intensity.Override(0.5f);
        }
    }
    
    public void TriggerMildEffects()
    {
        if (flashlight != null)
        {
            StartCoroutine(FlickerLight(0.4f));
        }
        
        if (vignette != null)
        {
            vignette.intensity.Override(0.3f);
        }
    }
    
    public void ClearEffects()
    {
        if (isFlickering)
        {
            StopAllCoroutines();
            isFlickering = false;
        }
        
        if (flashlight != null)
        {
            flashlight.intensity = 1f;
        }
        
        if (vignette != null)
        {
            vignette.intensity.Override(0f);
        }
        
        if (whisperSource != null && whisperSource.isPlaying)
        {
            whisperSource.Stop();
        }
    }
    
    private IEnumerator FlickerLight(float interval)
    {
        if (isFlickering) yield break;
        isFlickering = true;
        
        while (isFlickering)
        {
            flashlight.intensity = Random.Range(flickerIntensityMin, flickerIntensityMax);
            yield return new WaitForSeconds(interval);
        }
    }
}