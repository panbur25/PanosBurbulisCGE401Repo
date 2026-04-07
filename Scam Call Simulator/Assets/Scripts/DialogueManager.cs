using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text victimDialogueText;     // Harold's speech
    public Transform choicesContainer; // parent object for choice buttons
    public GameObject choiceButtonPrefab; // a Button prefab

    public void ShowStep(DialogueStep step)
    {
        Debug.Log("ShowStep called. choicesContainer is: " + choicesContainer);

        Debug.Log("Victim line: " + step.victimLine);
        Debug.Log("Number of choices: " + step.choices.Length);

        // Set Harold's line
        victimDialogueText.text = step.victimLine;

        // Clear old buttons
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        // Spawn a button for each choice
        foreach (DialogueChoice choice in step.choices)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
            btnObj.GetComponentInChildren<Text>().text = choice.text;

            // Capture choice in closure for the onClick listener
            DialogueChoice captured = choice;
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameManager.Instance.OnChoiceMade(captured);
            });
        }
    }

    public void DisableChoices()
    {
        foreach (Transform child in choicesContainer)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }
    }
}