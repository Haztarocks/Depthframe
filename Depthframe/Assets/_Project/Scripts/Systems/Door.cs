using UnityEngine;

public class Door : Interactable
{
    public DoorData doorData; // Reference to the scriptable object

    public Animator doorAnimator;

    protected override void Interact()
    {
        if (doorData.isLocked)
        {
            // Check if player has key
            InventorySystem inventory = Object.FindFirstObjectByType<InventorySystem>();
            if (inventory != null && inventory.HasItem(doorData.requiredKeyId)) // Use requiredKeyId
            {
                Unlock();
            }
            else
            {
                uiManager.ShowTooltip("Locked - Need " + doorData.requiredKeyId, transform.position); // Use requiredKeyId
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
        doorData.isLocked = false;
        uiManager.ShowTooltip("Door unlocked!", transform.position);
    }
}