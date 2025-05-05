using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Puzzle Data", menuName = "Interactables/Puzzle")]
public class PuzzleData : InteractableData
{
    [Serializable]
    public class PuzzleStep
    {
        public string description;
        public string hintText;
        public bool requiresItem;
        public string requiredItemId;
    }

    public string puzzleTitle;
    public PuzzleStep[] steps;
    public string completionMessage;
    public AudioClip successSound;
    public AudioClip failureSound;
}