using UnityEngine;
using TMPro;

public class Note : Interactable
{
    [TextArea(3, 10)]
    public string noteContent;
    public GameObject noteUI;
    public TextMeshProUGUI noteText;
    
    protected override void Start()
    {
        base.Start();
        if (noteUI != null)
            noteUI.SetActive(false);
    }
    
    protected override void Interact()
    {
        if (noteUI != null && noteText != null)
        {
            noteUI.SetActive(true);
            noteText.text = noteContent;
            onInteract.Invoke();
        }
    }
    
    public void CloseNote()
    {
        if (noteUI != null)
            noteUI.SetActive(false);
    }
}