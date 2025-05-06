using UnityEngine;
using PixelCrushers;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Save Settings")]
    public bool autoSaveOnExit = true;
    public int maxSaveSlots = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        if (autoSaveOnExit)
        {
            SaveGame(0); // Auto-save to slot 0
        }
    }

    public void SaveGame(int slot)
    {
        if (slot >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot: {slot}. Max slots: {maxSaveSlots}");
            return;
        }

        SaveSystem.SaveToSlot(slot);
        Debug.Log($"Game saved to slot {slot}");
    }

    public void LoadGame(int slot)
    {
        if (!DoesSaveExist(slot))
        {
            Debug.LogWarning($"No save file exists in slot {slot}");
            return;
        }

        SaveSystem.LoadFromSlot(slot);
        Debug.Log($"Game loaded from slot {slot}");
    }

    public bool DoesSaveExist(int slot)
    {
        return SaveSystem.HasSavedGameInSlot(slot);
    }

    public void DeleteSave(int slot)
    {
        if (DoesSaveExist(slot))
        {
            SaveSystem.DeleteSavedGameInSlot(slot);
            Debug.Log($"Deleted save in slot {slot}");
        }
    }

    public void QuickSave()
    {
        SaveGame(0);
    }

    public void QuickLoad()
    {
        LoadGame(0);
    }
}