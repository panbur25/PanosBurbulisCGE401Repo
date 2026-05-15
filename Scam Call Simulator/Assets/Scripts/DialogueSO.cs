using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "ScriptableObjects/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public string entryID;
    public bool isScammer; // <--- ADD THIS LINE
    public DialogueStepData[] steps;
    public string successEnding;
    public string failureEnding;
}