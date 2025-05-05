using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Battery UI")]
    public Image batteryFillImage;
    public TextMeshProUGUI batteryText;
    
    [Header("Sanity UI")]
    public Image sanityFillImage;
    public TextMeshProUGUI sanityText;
    
    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public float tooltipOffset = 20f;
    
    [Header("References")]
    public TorchBattery torchBattery;
    public SanitySystem sanitySystem;
    
    private void Start()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
    
    private void Update()
    {
        UpdateBatteryUI();
        UpdateSanityUI();
    }
    
    private void UpdateBatteryUI()
    {
        if (torchBattery != null)
        {
            float batteryPercentage = torchBattery.currentBattery / torchBattery.maxBattery;
            
            if (batteryFillImage != null)
                batteryFillImage.fillAmount = batteryPercentage;
                
            if (batteryText != null)
                batteryText.text = $"{Mathf.Round(batteryPercentage * 100)}%";
        }
    }
    
    private void UpdateSanityUI()
    {
        if (sanitySystem != null)
        {
            float sanityPercentage = sanitySystem.currentSanity / sanitySystem.maxSanity;
            
            if (sanityFillImage != null)
                sanityFillImage.fillAmount = sanityPercentage;
                
            if (sanityText != null)
                sanityText.text = $"Sanity: {Mathf.Round(sanityPercentage * 100)}%";
        }
    }
    
    public void ShowTooltip(string message, Vector2 position)
    {
        if (tooltipPanel != null && tooltipText != null)
        {
            tooltipText.text = message;
            tooltipPanel.SetActive(true);
            
            // Adjust position to follow mouse/interaction point
            Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
            tooltipPanel.transform.position = screenPos + Vector2.up * tooltipOffset;
        }
    }
    
    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}