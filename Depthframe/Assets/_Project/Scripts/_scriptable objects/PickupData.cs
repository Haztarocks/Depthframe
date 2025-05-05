using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "New Pickup Data", menuName = "Interactables/Pickup")]
public class PickupData : InteractableData
{
    public PickupType type;
    public float value;
    public string itemId;
    public Sprite itemSprite;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
}