using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "New Door Data", menuName = "Interactables/Door")]
public class DoorData : InteractableData
{
    public bool isLocked;
    public string requiredKeyId;
    public AudioClip lockedSound;
    public AudioClip unlockSound;
    public AnimationClip openAnimation;
    public AnimationClip closeAnimation;
}