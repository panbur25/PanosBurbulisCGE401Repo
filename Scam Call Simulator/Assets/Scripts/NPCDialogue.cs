using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 4f;
    public Transform player;
    public float typeSpeed = 0.02f;

    [Header("Data Source")]
    public DialogueStep currentStep; // The ScriptableObject asset

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject promptUI;

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactRange;

        if (promptUI != null) promptUI.SetActive(inRange && !isDialogueActive);

        // THE LOCK: If another script tries to force text in while we aren't typing, 
        // we wipe it. This kills the "Double Type" from the other script.
        if (!isTyping && isDialogueActive && dialogueText.text != "")
        {
            // Only do this if you find the other script is still winning
            // dialogueText.text = ""; 
        }

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive) OpenDialogue();
            else DisplayNext();
        }
    }

    public void OpenDialogue()
    {
        isDialogueActive = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        DisplayNext();
    }

    public void DisplayNext()
    {
        // 1. Grab the text from the ScriptableObject based on the scenario
        string targetText = "";

        if (GameManager.Instance != null)
        {
            targetText = (GameManager.Instance.CurrentScenario == NPCScenario.Scammer)
                         ? currentStep.assistantAdvice : currentStep.victimLine;
        }

        // 2. Start our typewriter
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(targetText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        // Give the other script a moment to finish its "Instant" write
        yield return new WaitForEndOfFrame();
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    public void CloseDialogue()
    {
        isDialogueActive = false;
        isTyping = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
    }
}