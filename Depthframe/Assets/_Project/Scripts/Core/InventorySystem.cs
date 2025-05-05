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
        }
    }

    public bool HasItem(string itemId)
    {
        return inventory.Contains(itemId);
    }

    public void RemoveItem(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
        {
            inventory.Remove(itemId);
        }
    }
}