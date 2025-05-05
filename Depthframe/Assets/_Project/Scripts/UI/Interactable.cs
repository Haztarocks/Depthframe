using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [Header("Basic Settings")]
    public string tooltipMessage = "Press E to interact";
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("Events")]
    public UnityEvent onInteract;
    
    protected UIManager uiManager;
    protected bool playerInRange;
    
    protected virtual void Start()
    {
        uiManager = Object.FindFirstObjectByType<UIManager>();
    }
    
    protected virtual void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (uiManager != null)
            {
                uiManager.ShowTooltip(tooltipMessage, transform.position);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (uiManager != null)
            {
                uiManager.HideTooltip();
            }
        }
    }
    
    protected abstract void Interact();
}