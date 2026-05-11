using System;
using UnityEngine;    // Added this for [Header]
using UnityEngine.UI; // Added this for Text

[Serializable]
public class DialogueStep
{
    public string victimLine;
    public string assistantAdvice;
    public DialogueChoice[] choices;

    [Header("Result Screen Feedback")]
    public string successFeedback;
    public string failureFeedback;
}