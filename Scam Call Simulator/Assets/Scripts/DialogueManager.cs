using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 1. Add this!

public class DialogueManager : MonoBehaviour
{
    [Header("TMP UI References")]
    public TextMeshProUGUI victimDialogueText; // 2. Change Text to TextMeshProUGUI
    public TextMeshProUGUI assistantAdviceText;

    public float typeSpeed = 0.05f;
    private Coroutine typingCoroutine;

    public Image assistantPortrait;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    [Header("Scenario Portraits")]
    public Sprite scammerAssistantSprite;
    public Sprite victimAssistantSprite;

    public void ShowStep(DialogueStepData step)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(DialogueSequence(step));

        if (assistantPortrait != null)
        {
            assistantPortrait.sprite = (GameManager.Instance.CurrentScenario == NPCScenario.Scammer)
                ? scammerAssistantSprite
                : victimAssistantSprite;
        }
    }

    IEnumerator DialogueSequence(DialogueStepData step) // Changed to DialogueStepData
    {
        // 1. Clear everything
        victimDialogueText.text = "";
        if (assistantAdviceText != null) assistantAdviceText.text = "";

        foreach (Transform child in choicesContainer) Destroy(child.gameObject);

        // 2. Type NPC Line (Scammer or Victim)
        yield return StartCoroutine(TypeText(victimDialogueText, step.npcLine));

        yield return new WaitForSeconds(0.5f);

        // 3. Type Assistant Advice (Wife or Coworker)
        if (assistantAdviceText != null)
        {
            // Use Ethan's wife for Victim mode, Coworker for Scammer mode
            string prefix = (GameManager.Instance.CurrentScenario == NPCScenario.Victim) ? "Wife: " : "Coworker: ";
            yield return StartCoroutine(TypeText(assistantAdviceText, prefix + step.assistantLine));
        }

        yield return new WaitForSeconds(0.3f);

        // 4. Simultaneous Choice Typing
        for (int i = 0; i < step.choices.Length; i++)
        {
            Choice choice = step.choices[i]; // Using our 'Choice' class
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesContainer);

            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            Button btn = btnObj.GetComponent<Button>();

            btn.interactable = false;

            Choice captured = choice;
            // This will pass the pointValue and type to your GameManager
            btn.onClick.AddListener(() => GameManager.Instance.OnChoiceMade(captured));

            StartCoroutine(TypeChoice(btnText, choice.text, btn));
        }

        typingCoroutine = null;
    }

    // 4. Update signatures to use TextMeshProUGUI
    IEnumerator TypeChoice(TextMeshProUGUI targetUI, string fullText, Button btn)
    {
        targetUI.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            targetUI.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        btn.interactable = true;
    }

    IEnumerator TypeText(TextMeshProUGUI targetUI, string fullText)
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