using UnityEngine;
using UnityEditor;


public class Pickup : Interactable
{
    public PickupData data;
    
    protected override void Start()
    {
        base.Start();
        if (data != null)
        {
            tooltipMessage = data.tooltipText;
        }
    }
    
    protected override void Interact()
    {
        if (data == null) return;
        
        switch (data.type)
        {
            case PickupType.Battery:
                TorchBattery battery = Object.FindFirstObjectByType<TorchBattery>();
                if (battery != null)
                {
                    battery.AddBattery(data.value);
                    Destroy(gameObject);
                }
                break;
                
            case PickupType.Key:
                InventorySystem inventory = Object.FindFirstObjectByType<InventorySystem>();
                if (inventory != null)
                {
                    inventory.AddItem(data.itemId);
                    Destroy(gameObject);
                }
                break;
                
            case PickupType.SanityItem:
                SanitySystem sanity = Object.FindFirstObjectByType<SanitySystem>();
                if (sanity != null)
                {
                    sanity.currentSanity += data.value;
                    Destroy(gameObject);
                }
                break;
                
            case PickupType.Document:
                onInteract.Invoke();
                Destroy(gameObject);
                break;
        }
    }
}
public enum PickupType
{
    Battery,
    Key,
    SanityItem,
    Document
}