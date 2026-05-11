using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text victimDialogueText;
    public float typeSpeed = 0.05f;
    private Coroutine typingCoroutine;

    public Text assistantAdviceText;    
    public Image assistantPortrait;  
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    [Header("Scenario Portraits")]
    public Sprite scammerAssistantSprite;
    public Sprite victimAssistantSprite;

    public void ShowStep(DialogueStep step)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Start the "Master Sequence" that handles both lines
        typingCoroutine = StartCoroutine(DialogueSequence(step));

        // Handle Portrait
        if (assistantPortrait != null)
        {
            assistantPortrait.sprite = (GameManager.Instance.CurrentScenario == NPCScenario.Scammer)
                ? scammerAssistantSprite
                : victimAssistantSprite;
        }
        /*
        // Choices (Keep your existing choice-clearing logic here)
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        foreach (DialogueChoice choice in step.choices)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
            btnObj.GetComponentInChildren<Text>().text = choice.text;
            DialogueChoice captured = choice;
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameManager.Instance.OnChoiceMade(captured);
            });
        }*/
    }

    IEnumerator DialogueSequence(DialogueStep step)
    {
        foreach (Transform child in choicesContainer) Destroy(child.gameObject);
        // 1. Clear everything first so the screen is clean
        victimDialogueText.text = "";
        if (assistantAdviceText != null) assistantAdviceText.text = "";

        // Clear choices container but keep the logic for buttons
        foreach (Transform child in choicesContainer) Destroy(child.gameObject);

        // 2. Type the Victim Line
        yield return StartCoroutine(TypeText(victimDialogueText, step.victimLine));

        // 3. Small Delay before Assistant
        yield return new WaitForSeconds(0.5f);

        if (assistantAdviceText != null)
        {
            yield return StartCoroutine(TypeText(assistantAdviceText, step.assistantAdvice));
        }

        // 4. Tiny delay before choices pop in
        yield return new WaitForSeconds(0.3f);

        // 5. Spawn and Type all choices SIMULTANEOUSLY
        for (int i = 0; i < step.choices.Length; i++)
        {
            DialogueChoice choice = step.choices[i];
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
            Text btnText = btnObj.GetComponentInChildren<Text>();
            Button btn = btnObj.GetComponent<Button>();

            // Disable button initially so they can't click until it's done typing
            btn.interactable = false;

            // Setup the button logic
            DialogueChoice captured = choice;
            btn.onClick.AddListener(() => GameManager.Instance.OnChoiceMade(captured));

            // Start typing this specific button text WITHOUT 'yield' 
            // This makes them all start at the same time!
            StartCoroutine(TypeChoice(btnText, choice.text, btn));
        }

        typingCoroutine = null;
    }

    IEnumerator TypeChoice(Text targetUI, string fullText, Button btn)
    {
        targetUI.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            targetUI.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        // Make the button clickable only after it's fully typed
        btn.interactable = true;
    }

    IEnumerator TypeText(Text targetUI, string fullText)
    {
        targetUI.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            targetUI.text += letter;
            yield return new WaitForSeconds(typeSpeed);
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