using System;

[Serializable]
public class DialogueChoice
{
    public string text;        // what the player sees
    public float trustDelta;   // +20, -25, etc.
    public bool isGood;        // counts toward good/bad pip
}