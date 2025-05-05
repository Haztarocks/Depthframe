using UnityEngine;

public class Door : Interactable
{
    public bool isLocked = false;
    public string keyRequired = "";
    public Animator doorAnimator;
    
    protected override void Interact()
    {
        if (isLocked)
        {
            // Check if player has key
            InventorySystem inventory = Object.FindFirstObjectByType<InventorySystem>();
            if (inventory != null && inventory.HasItem(keyRequired))
            {
                Unlock();
            }
            else
            {
                uiManager.ShowTooltip("Locked - Need " + keyRequired, transform.position);
            }
        }
        else
        {
            // Trigger door animation
            if (doorAnimator != null)
                doorAnimator.SetTrigger("Open");
            
            onInteract.Invoke();
        }
    }
    
    public void Unlock()
    {
        isLocked = false;
        uiManager.ShowTooltip("Door unlocked!", transform.position);
    }
}