using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    private HashSet<string> inventory = new HashSet<string>();

    public void AddItem(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
        {
            inventory.Add(itemId);
            Debug.Log($"Added item to inventory: {itemId}");
        }
    }

    public bool HasItem(string itemId)
    {
        bool hasItem = inventory.Contains(itemId);
        Debug.Log($"Checking inventory for item {itemId}: {hasItem}");
        return hasItem;
    }

    public void RemoveItem(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
        {
            inventory.Remove(itemId);
        }
    }
}