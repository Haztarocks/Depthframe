using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStealth : MonoBehaviour
{
    public float visibility; // 0 = invisible, 100 = fully visible
    public float lightDetectionRadius = 5f;
    public LayerMask lightLayerMask; // Layer for Light sources
    public float lightSensitivity = 1.0f; // Higher = more affected by light

    private SanitySystem sanitySystem;

    private void Start()
    {
        sanitySystem = GetComponent<SanitySystem>();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        Collider2D[] lights = Physics2D.OverlapCircleAll(transform.position, lightDetectionRadius, lightLayerMask);
        
        float lightAmount = 0f;

        foreach (var lightCollider in lights)
        {
            Light2D light = lightCollider.GetComponent<Light2D>();
            if (light != null)
            {
                float distance = Vector2.Distance(transform.position, light.transform.position);
                float intensityFactor = Mathf.Clamp01(1f - (distance / light.pointLightOuterRadius));
                lightAmount += light.intensity * intensityFactor;
            }
        }

        visibility = Mathf.Clamp(lightAmount * lightSensitivity * 100f, 0f, 100f);
    }

    // Called by EnemyAI when player is spotted in darkness
    public void OnSpottedInDarkness()
    {
        if (visibility < 30f && sanitySystem != null) // If player is mostly hidden
        {
            // Trigger sanity drop when spotted in darkness
            sanitySystem.currentSanity -= 10f; // Immediate sanity penalty for being spotted
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize light detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lightDetectionRadius);
    }
}
