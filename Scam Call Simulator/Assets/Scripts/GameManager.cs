using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum NPCScenario
{
    Scammer,
    Victim
}

public enum CallStatus
{
    Locked,
    Available,
    Scammed,
    HungUp
}

[System.Serializable]
public class NPCEntry
{
    public string npcName;
    public ScenarioData scammerScenario;
    public ScenarioData victimScenario;

    [HideInInspector] public CallStatus callStatus = CallStatus.Available;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("NPC Roster")]
    [SerializeField] private List<NPCEntry> npcRoster;

    private int currentNPCIndex = -1; // -1 = no active NPC (on contact screen)

    private NPCEntry CurrentNPC => npcRoster[currentNPCIndex];

    private const NPCScenario ACTIVE_SCENARIO = NPCScenario.Victim;
    //private const NPCScenario ACTIVE_SCENARIO = NPCScenario.Scammer;

    public NPCScenario CurrentScenario => ACTIVE_SCENARIO;

    private ScenarioData ActiveScenario =>
        CurrentScenario == NPCScenario.Victim
            ? CurrentNPC.victimScenario
            : CurrentNPC.scammerScenario;

    // Read-only access for PhoneUI to populate buttons
    public List<NPCEntry> NPCRoster => npcRoster;

    [Header("References")]
    public TrustBar trustBar;
    public DialogueManager dialogueManager;

    [Header("UI Panels")]
    public GameObject gamePanel;
    public GameObject resultsPanel;
    public GameObject phonePanel;   // The contact list screen
    public Text resultsTitleText;
    public Text resultsBodyText;

    [Header("Pip UI")]
    public Image[] goodPips;
    public Image[] badPips;

    [Header("Player")]
    public PlayerController playerController;

    [Header("Phone")]
    public PhoneInteractable phoneInteractable;
    public PhoneUI phoneUI;

    private int currentStep = 0;
    private int goodCount = 0;
    private int badCount = 0;
    private HashSet<int> metNPCs = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        // Reset all NPC statuses
        foreach (var npc in npcRoster)
            npc.callStatus = CallStatus.Available;

        metNPCs.Clear();

        currentNPCIndex = -1;
        currentStep = 0;
        goodCount = 0;
        badCount = 0;

        trustBar.SetTrust(50f);
        gamePanel.SetActive(false);
        resultsPanel.SetActive(false);
        phonePanel.SetActive(false);
        UpdatePips();
    }

    public void OnPlayerMetNPC(int npcIndex)
    {
        if (npcIndex < 0 || npcIndex >= npcRoster.Count) return;
        metNPCs.Add(npcIndex);
    }

    public CallStatus GetCallStatus(int npcIndex)
    {
        if (npcIndex < 0 || npcIndex >= npcRoster.Count) return CallStatus.Locked;
        if (!metNPCs.Contains(npcIndex)) return CallStatus.Locked;
        return npcRoster[npcIndex].callStatus;
    }

    // Called by PhoneInteractable when the player picks up the phone
    public void OpenPhone()
    {
        phonePanel.SetActive(true);
        playerController.enabled = false;
        phoneUI.RefreshContacts();
    }

    // Called by ContactButton when player taps a contact
    public void StartGameWithNPC(int index)
    {
        if (index < 0 || index >= npcRoster.Count)
        {
            Debug.LogWarning("[GameManager] Invalid NPC index: " + index);
            return;
        }

        if (npcRoster[index].callStatus != CallStatus.Available)
        {
            Debug.Log("[GameManager] NPC already called: " + npcRoster[index].npcName);
            return;
        }

        currentNPCIndex = index;
        currentStep = 0;
        goodCount = 0;
        badCount = 0;

        // CurrentScenario = ACTIVE_SCENARIO;
        trustBar.SetTrust(50f);
        UpdatePips();

        phonePanel.SetActive(false);
        gamePanel.SetActive(true);
        resultsPanel.SetActive(false);

        LoadStep(currentStep);
    }

    void LoadStep(int index)
    {
        if (index >= ActiveScenario.steps.Length)
        {
            EndGame(true);
            return;
        }
        dialogueManager.ShowStep(ActiveScenario.steps[index]);
    }

    public void OnChoiceMade(DialogueChoice choice)
    {
        dialogueManager.DisableChoices();
        trustBar.ModifyTrust(choice.trustDelta);

        if (choice.isGood) goodCount++;
        else badCount++;

        UpdatePips();
        CheckWinLose();
    }

    void CheckWinLose()
    {
        if (goodCount >= 5) { EndGame(true); return; }
        if (badCount >= 5) { EndGame(false); return; }

        if (goodCount >= 5 || badCount >= 5)
        {
            if (trustBar.TrustValue >= 100f) { EndGame(true); return; }
            if (trustBar.TrustValue <= 0f) { EndGame(false); return; }
        }

        currentStep++;
        Invoke("NextStep", 0.8f);
    }

    void NextStep()
    {
        LoadStep(currentStep);
    }

    void UpdatePips()
    {
        for (int i = 0; i < goodPips.Length; i++)
            goodPips[i].color = i < goodCount ? Color.green : Color.grey;

        for (int i = 0; i < badPips.Length; i++)
            badPips[i].color = i < badCount ? Color.red : Color.grey;
    }

    public void EndGame(bool win)
    {
        string npcName = CurrentNPC.npcName;

        // Mark the NPC's call status
        CurrentNPC.callStatus = win ? CallStatus.Scammed : CallStatus.HungUp;

        if (win)
        {
            resultsTitleText.text = "SCAM SUCCESSFUL!";
            resultsBodyText.text = $"{npcName} transferred the funds.\n\n" +
                "You built their trust slowly and struck at the right moment.\n" +
                "This is exactly how real IRS scams work.\n" +
                "TRUST FINAL: " + Mathf.RoundToInt(trustBar.TrustValue) + "%  |  " +
                "GOOD CHOICES: " + goodCount + "  |  BAD CHOICES: " + badCount;
        }
        else
        {
            resultsTitleText.text = "CALL DROPPED!";
            resultsBodyText.text = $"{npcName} got suspicious and hung up.\n\n" +
                "You pushed too hard, too fast.\n" +
                "Real victims hang up when something feels off.\n" +
                "TRUST FINAL: " + Mathf.RoundToInt(trustBar.TrustValue) + "%  |  " +
                "GOOD CHOICES: " + goodCount + "  |  BAD CHOICES: " + badCount;
        }

        gamePanel.SetActive(false);
        phoneInteractable.OnGameFinished();
        resultsPanel.SetActive(true);
    }

    // Called by the "Back to Contacts" button on the results panel
    public void CloseResultsToPhone()
    {
        resultsPanel.SetActive(false);

        // Check if all NPCs are done
        bool allDone = true;
        foreach (var npc in npcRoster)
        {
            if (npc.callStatus == CallStatus.Available)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            Debug.Log("[GameManager] All NPCs called. Game complete.");
            // TODO: hook up your game-complete screen here
            playerController.enabled = true;
        }
        else
        {
            phonePanel.SetActive(true);
            phoneUI.RefreshContacts();
        }
    }

    // Called by a "Hang Up / Back" button while on the phone screen
    public void ClosePhone()
    {
        phonePanel.SetActive(false);
        playerController.enabled = true;
        phoneInteractable.ClosePanel();
    }

    string GetFailureReason()
    {
        if (trustBar.TrustValue <= 0f)
            return $"you pushed too hard and {CurrentNPC.npcName} stopped believing you entirely.";
        if (badCount >= 5)
            return "too many aggressive or implausible choices broke their trust.";

        return "the approach raised too many red flags.";
    }

    public int GetNPCIndexByName(string name)
    {
        for (int i = 0; i < npcRoster.Count; i++)
        {
            if (npcRoster[i].npcName == name)
                return i;
        }
        Debug.LogWarning($"[GameManager] No NPC found with name: {name}");
        return -1;
    }
}