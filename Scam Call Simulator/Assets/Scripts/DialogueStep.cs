using System;

[Serializable]
public class DialogueStep
{
    public string victimLine;           // what Harold says
    public DialogueChoice[] choices;    // 2-3 choices for this step
}