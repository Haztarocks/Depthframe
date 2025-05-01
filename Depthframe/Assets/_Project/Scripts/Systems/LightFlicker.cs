using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.1f;

    private Light2D light2D;
    private float baseIntensity;
    private float flickerTimer;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D != null)
            baseIntensity = light2D.intensity;
    }

    void Update()
    {
        if (light2D == null) return;

        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            float randomIntensity = baseIntensity * Random.Range(minIntensity, maxIntensity);
            light2D.intensity = randomIntensity;
            flickerTimer = flickerSpeed;
        }
    }
}