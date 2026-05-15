using System.Collections.Generic;
using UnityEngine;
using TMPro; // Added for TextMeshPro support

public class PhoneUI : MonoBehaviour
{
    [Header("Contact List")]
    [SerializeField] private Transform contactListParent;
    [SerializeField] private GameObject contactButtonPrefab;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerText; // Changed to TMP

    private List<ContactButton> spawnedButtons = new List<ContactButton>();

    // This ensures it refreshes every time you open the phone
    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        RefreshContacts();
    }

    public void RefreshContacts()
    {
        // Clear existing buttons
        foreach (var btn in spawnedButtons)
            if (btn != null) Destroy(btn.gameObject);

        spawnedButtons.Clear();

        List<NPCEntry> roster = GameManager.Instance.NPCRoster;
        LevelData levelData = GameManager.Instance.CurrentLevelData;

        // Header text logic
        int remaining = 0;
        foreach (int idx in levelData.npcRosterIndices)
            if (roster[idx].callStatus == CallStatus.Available)
                remaining++;

        if (headerText != null)
            headerText.text = $"LEVEL {GameManager.Instance.CurrentLevelIndex + 1}\n<size=80%>{remaining} call(s) remaining</size>";

        // Spawn buttons for the current level
        foreach (int idx in levelData.npcRosterIndices)
        {
            GameObject go = Instantiate(contactButtonPrefab, contactListParent);
            ContactButton btn = go.GetComponent<ContactButton>();
            if (btn != null)
            {
                btn.Setup(idx, roster[idx].npcName,
                    GameManager.Instance.GetCallStatus(idx),
                    roster[idx].profilePic,
                    roster[idx].contactDescription);

                spawnedButtons.Add(btn);
            }
        }
    }
}