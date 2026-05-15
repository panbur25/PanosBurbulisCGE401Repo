using UnityEngine;

[System.Serializable]
public class Choice
{
    public string text;
    public string type;
    public int pointValue; // We'll use this as trustDelta
}

[System.Serializable]
public class DialogueStepData
{
    public string npcLine;
    public string assistantLine;
    public Choice[] choices;
}

[System.Serializable]
public class DialogueContainer // This matches the individual entry in your JSON
{
    public string entryID;
    public bool isScammer; // <--- ADD THIS LINE
    public DialogueStepData[] steps;
    public string successEnding;
    public string failureEnding;
}

[System.Serializable]
public class DialogueListWrapper
{
    public DialogueContainer[] allDialogues;
}