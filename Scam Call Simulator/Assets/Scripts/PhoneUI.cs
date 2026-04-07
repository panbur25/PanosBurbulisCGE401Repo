using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    [Header("Contact List")]
    [SerializeField] private Transform contactListParent; // The scroll view content object
    [SerializeField] private GameObject contactButtonPrefab; // ContactButton prefab

    [Header("Header")]
    [SerializeField] private Text headerText;

    private List<ContactButton> spawnedButtons = new List<ContactButton>();

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        RefreshContacts();
    }

    // Populates or refreshes the contact list from the GameManager roster
    public void RefreshContacts()
    {
        // Clear old buttons
        foreach (var btn in spawnedButtons)
        {
            if (btn != null)
                Destroy(btn.gameObject);
        }
        spawnedButtons.Clear();

        List<NPCEntry> roster = GameManager.Instance.NPCRoster;

        // Count remaining using GetCallStatus so metNPCs is respected
        int remaining = 0;
        foreach (var npc in roster)
            if (npc.callStatus != CallStatus.Scammed && npc.callStatus != CallStatus.HungUp)
                remaining++;

        if (headerText != null)
            headerText.text = $"CONTACTS  ({remaining} remaining)";

        // Spawn a button for each NPC
        for (int i = 0; i < roster.Count; i++)
        {
            GameObject go = Instantiate(contactButtonPrefab, contactListParent);
            ContactButton btn = go.GetComponent<ContactButton>();

            if (btn != null)
            {
                // GetCallStatus instead of roster[i].callStatus
                btn.Setup(i, roster[i].npcName, GameManager.Instance.GetCallStatus(i));
                spawnedButtons.Add(btn);
            }
        }
    }
}