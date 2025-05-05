using UnityEngine;
using UnityEngine.Events;

public class PuzzleTerminal : Interactable
{
    [System.Serializable]
    public class PuzzleStep
    {
        public string description;
        public UnityEvent onStepComplete;
        public bool isCompleted;
    }
    
    public PuzzleStep[] steps;
    public GameObject puzzleUI;
    private int currentStep = 0;
    
    protected override void Start()
    {
        base.Start();
        if (puzzleUI != null)
            puzzleUI.SetActive(false);
    }
    
    protected override void Interact()
    {
        if (puzzleUI != null)
        {
            puzzleUI.SetActive(true);
            UpdatePuzzleUI();
        }
    }
    
    private void UpdatePuzzleUI()
    {
        // Update UI to show current step and progress
        if (currentStep < steps.Length)
        {
            uiManager.ShowTooltip(steps[currentStep].description, transform.position);
        }
    }
    
    public void CompleteCurrentStep()
    {
        if (currentStep < steps.Length)
        {
            steps[currentStep].isCompleted = true;
            steps[currentStep].onStepComplete.Invoke();
            currentStep++;
            
            if (currentStep >= steps.Length)
            {
                onInteract.Invoke(); // Puzzle completed
                puzzleUI.SetActive(false);
            }
            else
            {
                UpdatePuzzleUI();
            }
        }
    }
}