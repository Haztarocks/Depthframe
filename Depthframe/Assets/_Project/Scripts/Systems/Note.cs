using UnityEngine;
using TMPro;
using System.Collections;

public class Note : Interactable
{
    public NoteData noteData; // Add this line to reference the scriptable object

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
            noteText.text = noteData.noteContent; // Use noteData to get the content
            onInteract.Invoke();
            StartCoroutine(FadeOutNoteUI());
        }
    }
    
    private IEnumerator FadeOutNoteUI()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds
        if (noteUI != null)
            noteUI.SetActive(false);
    }
    
    public void CloseNote()
    {
        if (noteUI != null)
            noteUI.SetActive(false);
    }
}