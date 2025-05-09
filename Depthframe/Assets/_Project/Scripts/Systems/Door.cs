using UnityEngine;
using UnityEngine.SceneManagement; // Add this line to use SceneManager

public class Door : Interactable
{
    public DoorData doorData; // Reference to the scriptable object

    public Animator doorAnimator;
    public string sceneToLoad; // Add this line to specify the scene name

    protected override void Interact()
    {
        if (doorData.isLocked)
        {
            // Check if player has key
            InventorySystem inventory = Object.FindFirstObjectByType<InventorySystem>();
            if (inventory != null && inventory.HasItem(doorData.requiredKeyId))
            {
                Unlock();
            }
            else
            {
                uiManager.ShowTooltip("Locked - Need " + doorData.requiredKeyId, transform.position);
            }
        }
        else
        {
            // Trigger door animation
            if (doorAnimator != null)
                doorAnimator.SetTrigger("Open");
            
            onInteract.Invoke();
            
            // Load the specified scene
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad); // Correct usage
            }
        }
    }

    public void Unlock()
    {
        doorData.isLocked = false;
        uiManager.ShowTooltip("Door unlocked!", transform.position);
    }
}