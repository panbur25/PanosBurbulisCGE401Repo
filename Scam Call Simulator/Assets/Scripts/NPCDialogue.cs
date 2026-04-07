using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [Header("Interaction")]
    public float interactRange = 3f;
    public Transform player;

    [Header("UI")]
    public GameObject promptUI;      // "Press E" prompt
    public GameObject dialogueBubble; // the speech bubble panel
    public Text dialogueText;

    [Header("Dialogue")]
    [TextArea] public string hint; // type your hint directly in the Inspector

    [Header("NPC Identity")]
    [SerializeField] private string npcName;

    private bool bubbleOpen = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= interactRange;

        // Show prompt only when in range and bubble isn't open
        promptUI.SetActive(inRange && !bubbleOpen);

        // Open on E
        if (inRange && !bubbleOpen && Input.GetKeyDown(KeyCode.E))
            OpenBubble();

        // Close on X
        if (bubbleOpen && Input.GetKeyDown(KeyCode.X))
            CloseBubble();

        // Close when walking away
        if (bubbleOpen && !inRange)
            CloseBubble();
    }

    void OpenBubble()
    {
        dialogueText.text = hint;
        dialogueBubble.SetActive(true);
        promptUI.SetActive(false);
        bubbleOpen = true;

        int index = GameManager.Instance.GetNPCIndexByName(npcName); // NEW
        if (index >= 0)
            GameManager.Instance.OnPlayerMetNPC(index);
    }

    public void CloseBubble()
    {
        dialogueBubble.SetActive(false);
        bubbleOpen = false;
    }
}