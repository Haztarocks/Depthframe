using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity;
    public float sanityDrainRate = 5f;
    public float sanityRestoreRate = 3f;
    public float enemyEncounterSanityLoss = 10f; // Add this line

    [Header("References")]
    public Light2D playerLight;

    public bool IsInLight => playerLight != null && playerLight.intensity > 0.1f;

    public delegate void OnSanityChanged(float sanity);
    public static event OnSanityChanged SanityChanged;

    private void Start()
    {
        currentSanity = maxSanity;

        if (playerLight == null)
        {
            Debug.LogError("Player light reference is missing in SanitySystem!");
        }
    }

    private void Update()
    {
        if (playerLight == null) return;
        float delta = Time.deltaTime * (IsInLight ? sanityRestoreRate : -sanityDrainRate);
        currentSanity = Mathf.Clamp(currentSanity + delta, 0, maxSanity);

        SanityChanged?.Invoke(currentSanity);

        HandleSanityEffects();

        // Check for game over condition
        if (currentSanity <= 0)
        {
            Debug.Log("Sanity reached zero, loading GameOver scene.");
            SceneManager.Instance.LoadScene("GameOver", true);
                    Debug.LogError("SceneManager instance is null.");
        }
    }

    public void OnEnemyEncounter()
    {
        currentSanity = Mathf.Clamp(currentSanity - enemyEncounterSanityLoss, 0, maxSanity);
        SanityChanged?.Invoke(currentSanity);
    }

    void HandleSanityEffects()
    {
        var traumaManager = TraumaManager.Instance;
        if (traumaManager == null) return;

        if (currentSanity < 25)
        {
            traumaManager.TriggerCriticalEffects();
        }
        else if (currentSanity < 50)
        {
            traumaManager.TriggerMediumEffects();
        }
        else if (currentSanity < 75)
        {
            traumaManager.TriggerMildEffects();
        }
        else
        {
            traumaManager.ClearEffects();
        }
    }
}