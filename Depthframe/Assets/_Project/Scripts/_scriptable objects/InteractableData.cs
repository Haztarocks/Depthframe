using UnityEngine;

public abstract class InteractableData : ScriptableObject
{
    public string tooltipText;
    public string interactionPrompt;
    public AudioClip interactionSound;
    public float interactionRadius = 2f;
}