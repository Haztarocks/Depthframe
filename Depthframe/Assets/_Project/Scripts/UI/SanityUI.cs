using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanityUI : MonoBehaviour
{
    [Header("UI References")]
    public Image sanityBar;
    public TextMeshProUGUI sanityText;
    
    private void OnEnable()
    {
        SanitySystem.SanityChanged += UpdateSanityUI;
    }
    
    private void OnDisable()
    {
        SanitySystem.SanityChanged -= UpdateSanityUI;
    }
    
    private void UpdateSanityUI(float currentSanity)
    {
        if (sanityBar != null)
            sanityBar.fillAmount = currentSanity / 100f;
            
        if (sanityText != null)
            sanityText.text = $"Sanity: {Mathf.Round(currentSanity)}%";
    }
}