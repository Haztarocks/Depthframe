using UnityEngine;
using UnityEngine.UIElements;

public class SaveMenuUIToolkit : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset saveSlotTemplate;
    [SerializeField] private int maxSaveSlots = 5;
    
    private UIDocument document;
    private VisualElement root;
    private VisualElement slotsContainer;
    
    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        if (document == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }

        root = document.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root VisualElement not found!");
            return;
        }

        slotsContainer = root.Q<VisualElement>("slots-container");
        if (slotsContainer == null)
        {
            Debug.LogError("Slots container not found! Make sure your UXML has an element with name=\"slots-container\"");
            return;
        }

        if (saveSlotTemplate == null)
        {
            Debug.LogError("Save slot template not assigned!");
            return;
        }

        // Add close button handler
        var closeButton = root.Q<Button>("close-button");
        if (closeButton != null)
        {
            closeButton.clicked += Hide;
        }

        InitializeSaveSlots();
    }

    private void OnDisable()
    {
        if (root != null)
        {
            var closeButton = root.Q<Button>("close-button");
            if (closeButton != null)
            {
                closeButton.clicked -= Hide;
            }
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        RefreshAllSlots();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void InitializeSaveSlots()
    {
        if (slotsContainer == null || saveSlotTemplate == null) return;

        // Check if SaveManager exists
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager instance not found! Make sure SaveManager exists in the scene.");
            return;
        }

        // Clear existing slots
        slotsContainer.Clear();

        for (int i = 0; i < maxSaveSlots; i++)
        {
            var slotInstance = saveSlotTemplate.Instantiate();
            if (slotInstance == null) continue;

            var slotRoot = slotInstance.Q<VisualElement>("save-slot");
            if (slotRoot == null)
            {
                Debug.LogError($"Save slot root element not found in template! Make sure your SaveSlot.uxml has an element with name=\"save-slot\"");
                continue;
            }
            
            int slotIndex = i;
            var slotNumber = slotRoot.Q<Label>("slot-number");
            if (slotNumber != null) slotNumber.text = $"Slot {i + 1}";
            
            var saveButton = slotRoot.Q<Button>("save-button");
            var loadButton = slotRoot.Q<Button>("load-button");
            var deleteButton = slotRoot.Q<Button>("delete-button");
            var saveInfo = slotRoot.Q<Label>("save-info");
            
            if (saveButton != null)
            {
                saveButton.clicked += () => {
                    SaveManager.Instance?.SaveGame(slotIndex);
                    RefreshSlotState(slotIndex, saveInfo, loadButton, deleteButton);
                };
            }
            
            if (loadButton != null)
            {
                loadButton.clicked += () => SaveManager.Instance?.LoadGame(slotIndex);
            }
            
            if (deleteButton != null)
            {
                deleteButton.clicked += () => {
                    SaveManager.Instance?.DeleteSave(slotIndex);
                    RefreshSlotState(slotIndex, saveInfo, loadButton, deleteButton);
                };
            }
            
            RefreshSlotState(slotIndex, saveInfo, loadButton, deleteButton);
            slotsContainer.Add(slotInstance);
        }
    }
    
    private void RefreshAllSlots()
    {
        if (slotsContainer == null) return;
        
        foreach (var element in slotsContainer.Children())
        {
            var slotRoot = element.Q<VisualElement>("save-slot");
            if (slotRoot == null) continue;

            var saveInfo = slotRoot.Q<Label>("save-info");
            var loadBtn = slotRoot.Q<Button>("load-button");
            var deleteBtn = slotRoot.Q<Button>("delete-button");
            
            // Get slot index from the slot number text
            var slotNumber = slotRoot.Q<Label>("slot-number");
            if (slotNumber != null)
            {
                string numberText = slotNumber.text.Replace("Slot ", "");
                if (int.TryParse(numberText, out int index))
                {
                    RefreshSlotState(index - 1, saveInfo, loadBtn, deleteBtn);
                }
            }
        }
    }
    
    private void RefreshSlotState(int index, Label infoLabel, Button loadBtn, Button deleteBtn)
    {
        if (infoLabel == null || loadBtn == null || deleteBtn == null || SaveManager.Instance == null) return;
        
        bool saveExists = SaveManager.Instance.DoesSaveExist(index);
        loadBtn.SetEnabled(saveExists);
        deleteBtn.SetEnabled(saveExists);
        infoLabel.text = saveExists ? "Save Data Exists" : "Empty Slot";
    }
}