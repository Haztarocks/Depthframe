using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SaveMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject saveMenuPanel;
    public Transform saveSlotContainer;
    public GameObject saveSlotPrefab;
    public Button closeButton;
    
    [Header("Settings")]
    public int displayedSlots = 5;
    
    private List<SaveSlotUI> saveSlots = new List<SaveSlotUI>();
    
    private void Start()
    {
        closeButton.onClick.AddListener(HideSaveMenu);
        InitializeSaveSlots();
        HideSaveMenu();
    }
    
    private void InitializeSaveSlots()
    {
        for (int i = 0; i < displayedSlots; i++)
        {
            GameObject slotObj = Instantiate(saveSlotPrefab, saveSlotContainer);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();
            if (slotUI != null)
            {
                slotUI.Initialize(i);
                saveSlots.Add(slotUI);
            }
        }
    }
    
    public void ShowSaveMenu()
    {
        saveMenuPanel.SetActive(true);
        RefreshSlots();
    }
    
    public void HideSaveMenu()
    {
        saveMenuPanel.SetActive(false);
    }
    
    public void RefreshSlots()
    {
        foreach (var slot in saveSlots)
        {
            slot.RefreshSlotState();
        }
    }
}