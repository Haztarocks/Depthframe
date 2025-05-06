using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public float tooltipOffset = 1f;
    
    [Header("Save System")]
    public SaveMenuUI saveMenuUI;            // Legacy UI version
    public SaveMenuUIToolkit saveMenuToolkit; // UI Toolkit version
    
    private void Update()
    {
        // Add a key to toggle the save menu (e.g., Escape)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSaveMenu();
        }
    }
    
    public void ToggleSaveMenu()
    {
        // Try UI Toolkit version first
        if (saveMenuToolkit != null)
        {
            if (saveMenuToolkit.gameObject.activeSelf)
            {
                saveMenuToolkit.Hide();
            }
            else
            {
                saveMenuToolkit.Show();
            }
        }
        // Fall back to legacy UI if UI Toolkit version is not available
        else if (saveMenuUI != null)
        {
            if (saveMenuUI.gameObject.activeSelf)
            {
                saveMenuUI.HideSaveMenu();
            }
            else
            {
                saveMenuUI.ShowSaveMenu();
            }
        }
    }
    
    public void ShowTooltip(string message, Vector3 position)
    {
        if (tooltipPanel != null && tooltipText != null)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = message;
            
            // Adjust position to be above the object
            Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
            screenPos.y += tooltipOffset;
            tooltipPanel.transform.position = screenPos;
        }
    }
    
    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}