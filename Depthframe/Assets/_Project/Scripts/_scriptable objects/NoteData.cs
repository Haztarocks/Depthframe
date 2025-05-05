using UnityEngine;

[CreateAssetMenu(fileName = "New Note Data", menuName = "Interactables/Note")]
public class NoteData : InteractableData
{
    [TextArea(3, 10)]
    public string noteContent;
    public Sprite noteImage;
    public bool useTypewriterEffect;
    public float typewriterSpeed = 0.05f;
}