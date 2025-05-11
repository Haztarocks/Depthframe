using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity;
    public float sanityDrainRate = 5f;
    public float sanityRestoreRate = 3f;
    public float enemyEncounterSanityLoss = 10f;

    [Header("References")]
    public Light2D playerLight;
    
    private PlayerController playerController;
    private bool isGameOverTriggered = false;

    public bool IsInLight => playerLight != null && playerLight.intensity > 0.1f;

    public delegate void OnSanityChanged(float sanity);
    public static event OnSanityChanged SanityChanged;

    private void Start()
    {
        currentSanity = maxSanity;
        playerController = GetComponentInParent<PlayerController>();

        if (playerLight == null)
        {
            Debug.LogError("Player light reference is missing in SanitySystem!");
        }
    }

    private void Update()
    {
        if (playerLight == null) return;
        if (isGameOverTriggered) return;
        
        // Only restore sanity if torch is on, otherwise always drain
        bool torchIsOn = false;
        if (playerController != null && playerController.torchLight != null)
        {
            torchIsOn = playerController.torchLight.enabled;
        }
        
        // Always drain sanity unless torch is on AND player is in light
        float delta = Time.deltaTime * (torchIsOn && IsInLight ? sanityRestoreRate : -sanityDrainRate);
        currentSanity = Mathf.Clamp(currentSanity + delta, 0, maxSanity);

        SanityChanged?.Invoke(currentSanity);

        HandleSanityEffects();

        // Check for game over condition
        if (currentSanity <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if (isGameOverTriggered) return; // Prevent multiple calls
        
        isGameOverTriggered = true;
        Debug.Log("Sanity reached zero, loading GameOver scene.");
        
        if (SceneManager.Instance != null)
        {
            // Try to load the scene directly if SceneManager instance is available
            SceneManager.Instance.LoadScene("GameOver", true);
        }
        else
        {
            // Fallback to Unity's SceneManager if our custom one is null
            Debug.LogWarning("Custom SceneManager instance is null, using Unity SceneManager instead.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
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