using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BatteryUI : MonoBehaviour
{
    [Header("UI References")]
    public Image batteryBar;
    public TextMeshProUGUI batteryText;
    public Image batteryImage;
    public Sprite highBatterySprite;
    public Sprite mediumBatterySprite;
    public Sprite lowBatterySprite;

    private void OnEnable()
    {
        TorchBattery.BatteryChanged += UpdateBatteryUI;
    }
    
    private void OnDisable()
    {
        TorchBattery.BatteryChanged -= UpdateBatteryUI;
    }
    
    private void UpdateBatteryUI(float currentBattery)
    {
        if (batteryBar != null)
            batteryBar.fillAmount = currentBattery / 100f;
            
        if (batteryText != null)
            batteryText.text = $"Battery: {Mathf.Round(currentBattery)}%";

        if (batteryImage != null)
        {
            if (currentBattery >= 75)
            {
                batteryImage.sprite = highBatterySprite;
            }
            else if (currentBattery >= 50)
            {
                batteryImage.sprite = mediumBatterySprite;
            }
            else
            {
                batteryImage.sprite = lowBatterySprite;
            }
        }
    }
}