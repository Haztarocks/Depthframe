using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotNumberText;
    public TextMeshProUGUI saveInfoText;
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton;
    
    private int slotIndex;
    
    public void Initialize(int index)
    {
        slotIndex = index;
        slotNumberText.text = $"Slot {index + 1}";
        
        saveButton.onClick.AddListener(() => SaveGame());
        loadButton.onClick.AddListener(() => LoadGame());
        deleteButton.onClick.AddListener(() => DeleteSave());
        
        RefreshSlotState();
    }
    
    public void RefreshSlotState()
    {
        bool saveExists = SaveManager.Instance.DoesSaveExist(slotIndex);
        loadButton.interactable = saveExists;
        deleteButton.interactable = saveExists;
        saveInfoText.text = saveExists ? "Save Data Exists" : "Empty Slot";
    }
    
    private void SaveGame()
    {
        SaveManager.Instance.SaveGame(slotIndex);
        RefreshSlotState();
    }
    
    private void LoadGame()
    {
        SaveManager.Instance.LoadGame(slotIndex);
    }
    
    private void DeleteSave()
    {
        SaveManager.Instance.DeleteSave(slotIndex);
        RefreshSlotState();
    }
}