using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro; // 1. Add this!

public enum NPCScenario { Scammer, Victim }
public enum CallStatus { Locked, Available, Scammed, HungUp }

[System.Serializable]
public class NPCEntry
{
    public string npcName;
    public string contactDescription;
    public Sprite profilePic;
    // Update these to use DialogueSO (the ScriptableObject we created)
    public DialogueSO scammerScenario;
    public DialogueSO victimScenario;
    [HideInInspector] public CallStatus callStatus = CallStatus.Available;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("NPC Roster (all 20 NPCs)")]
    [SerializeField] private List<NPCEntry> npcRoster;

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;
    private int currentLevelIndex = 0;

    [Header("Scenario")]
    [SerializeField] private NPCScenario activeScenario = NPCScenario.Victim;
    public NPCScenario CurrentScenario => activeScenario;

    private int currentNPCIndex = -1;
    private NPCEntry CurrentNPC => npcRoster[currentNPCIndex];

    public List<NPCEntry> NPCRoster => npcRoster;
    public LevelData CurrentLevelData => levels[currentLevelIndex];
    public int CurrentLevelIndex => currentLevelIndex;

    [Header("Scene Objects (Scenario-Dependent)")]
    public GameObject scammerObjects;
    public GameObject victimObjects;

    [Header("References")]
    public TrustBar trustBar;
    public DialogueManager dialogueManager;

    [Header("UI Panels")]
    public GameObject gamePanel;
    public GameObject resultsPanel;
    public TextMeshProUGUI resultsTitleText; // 2. Update these!
    public TextMeshProUGUI resultsBodyText;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteTitleText;
    public TextMeshProUGUI levelCompleteBodyText;

    [Header("Game Panel NPC Info")]
    public Image gameProfileImage;
    public TextMeshProUGUI gameInfoText;

    [Header("Pip UI")]
    public Image[] goodPips;
    public Image[] badPips;

    [Header("Player")]
    public PlayerController playerController;
    public Transform playerSpawnPoint;

    [Header("Phone")]
    //public PhoneInteractable phoneInteractable;
    public PhoneUI scammerPhoneUI;
    public PhoneUI victimPhoneUI;
    public GameObject scammerPhonePanel;
    public GameObject victimPhonePanel;

    private GameObject ActivePhonePanel =>
        activeScenario == NPCScenario.Scammer ? scammerPhonePanel : victimPhonePanel;

    private PhoneUI ActivePhoneUI =>
        activeScenario == NPCScenario.Scammer ? scammerPhoneUI : victimPhoneUI;

    private int currentStep = 0;
    private int goodCount = 0;
    private int badCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() => InitGame();

    public void InitGame()
    {
        ApplyScenarioScene();

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        foreach (var npc in npcRoster)
            npc.callStatus = CallStatus.Available;

        currentLevelIndex = 0;
        currentNPCIndex = -1;
        currentStep = 0;
        goodCount = 0;
        badCount = 0;

        trustBar.SetTrust(50f);
        gamePanel.SetActive(false);
        resultsPanel.SetActive(false);
        ActivePhonePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        UpdatePips();
    }

    void ApplyScenarioScene()
    {
        bool isScammer = activeScenario == NPCScenario.Scammer;
        if (scammerObjects != null) scammerObjects.SetActive(isScammer);
        if (victimObjects != null) victimObjects.SetActive(!isScammer);
    }

    private DialogueSO ActiveScenario =>
    CurrentScenario == NPCScenario.Victim
        ? CurrentNPC.victimScenario
        : CurrentNPC.scammerScenario;

    public CallStatus GetCallStatus(int npcIndex)
    {
        if (npcIndex < 0 || npcIndex >= npcRoster.Count) return CallStatus.Locked;
        if (!IsNPCInCurrentLevel(npcIndex)) return CallStatus.Locked;
        return npcRoster[npcIndex].callStatus;
    }

    public bool IsNPCInCurrentLevel(int npcIndex)
    {
        if (levels == null || currentLevelIndex >= levels.Length) return false;
        foreach (int idx in levels[currentLevelIndex].npcRosterIndices)
            if (idx == npcIndex) return true;
        return false;
    }

    public void OpenPhone()
    {
        ActivePhonePanel.SetActive(true);
        playerController.FreezePlayer();
        playerController.enabled = false;
        ActivePhoneUI.RefreshContacts();
    }

    public void StartGameWithNPC(int index)
    {
        if (index < 0 || index >= npcRoster.Count) return;
        if (npcRoster[index].callStatus != CallStatus.Available) return;

        currentNPCIndex = index;
        currentStep = 0;
        goodCount = 0;
        badCount = 0;

        //if (gameProfileImage != null) gameProfileImage.sprite = npcRoster[index].profilePic;
        if (gameInfoText != null) gameInfoText.text = npcRoster[index].npcName;

        trustBar.SetTrust(50f);
        trustBar.ResetDelta();
        UpdatePips();

        ActivePhonePanel.SetActive(false);
        gamePanel.SetActive(true);
        resultsPanel.SetActive(false);

        LoadStep(currentStep);
    }

    void LoadStep(int index)
    {
        if (index >= ActiveScenario.steps.Length) { EndGame(true); return; }
        dialogueManager.ShowStep(ActiveScenario.steps[index]);
    }

    public void OnChoiceMade(Choice choice)
    {
        dialogueManager.DisableChoices();

        // Handle the Toss-up logic
        int finalDelta = choice.pointValue;
        if (choice.type == "tossup")
        {
            finalDelta = (currentStep % 2 == 0) ? Random.Range(1, 6) : Random.Range(-5, 0);
        }

        trustBar.ModifyTrust(finalDelta);

        // Track "Good" vs "Bad" for your pips
        if (choice.type == "good") goodCount++;
        else if (choice.type == "bad") badCount++;

        UpdatePips();

        // Wait a bit before moving to next step so player sees the trust bar move
        currentStep++;
        Invoke("NextStep", 1.2f);
    }

    /*
    void CheckWinLose()
    {
        if (goodCount >= 5) { EndGame(true); return; }
        if (badCount >= 5) { EndGame(false); return; }
        if (trustBar.TrustValue >= 100f) { EndGame(true); return; }
        if (trustBar.TrustValue <= 0f) { EndGame(false); return; }
        currentStep++;
        Invoke("NextStep", 0.8f);
    } */

    void NextStep()
    {
        bool isVictimScenario = CurrentScenario == NPCScenario.Victim;

        if (isVictimScenario)
        {
            // ==========================================
            // VICTIM SCENARIO RULESET
            // ==========================================
            // Win: You successfully hang up / derail the scammer
            if (goodCount >= 5 || trustBar.TrustValue >= 100f)
            {
                EndGame(true);
                return;
            }

            // Lose: Scammer tricks you / you fall for it
            if (badCount >= 5 || trustBar.TrustValue <= 0f)
            {
                EndGame(false);
                return;
            }
        }
        else
        {
            // ==========================================
            // SCAMMER SCENARIO RULESET
            // ==========================================
            // Win: You successfully trick the victim
            if (goodCount >= 5 || trustBar.TrustValue >= 100f)
            {
                EndGame(true);
                return;
            }

            // Lose: Victim hangs up / you blow it
            if (badCount >= 5 || trustBar.TrustValue <= 0f)
            {
                EndGame(false);
                return;
            }
        }

        // ==========================================
        // Continue to next step if no win/loss yet
        // ==========================================
        if (currentStep >= ActiveScenario.steps.Length)
        {
            // Ran out of dialogue steps - decide winner by trust bar position
            bool playerWon = isVictimScenario
                ? trustBar.TrustValue >= 50f   // Victim: high suspicion = you won
                : trustBar.TrustValue >= 50f;  // Scammer: high trust = you won
            EndGame(playerWon);
        }
        else
        {
            LoadStep(currentStep);
        }
    }

    void UpdatePips()
    {
        bool isScammer = activeScenario == NPCScenario.Scammer;

        Color goodColor = isScammer ? Color.green : Color.green; // Usually players like green for "success"
        Color badColor = isScammer ? Color.red : Color.red;

        for (int i = 0; i < goodPips.Length; i++)
            goodPips[i].color = i < goodCount ? goodColor : Color.grey;

        for (int i = 0; i < badPips.Length; i++)
            badPips[i].color = i < badCount ? badColor : Color.grey;
    }

    public void EndGame(bool win)
    {
        string npcName = CurrentNPC.npcName;
        CurrentNPC.callStatus = win ? CallStatus.Scammed : CallStatus.HungUp;

        bool isVictimScenario = CurrentScenario == NPCScenario.Victim;

        if (isVictimScenario)
        {
            // VICTIM SCENARIO TEXT
            resultsTitleText.text = win ? "YOU SPOTTED THE SCAM!" : "YOU GOT SCAMMED!";
        }
        else
        {
            // SCAMMER SCENARIO TEXT
            resultsTitleText.text = win ? "SCAM SUCCESSFUL!" : "CALL DROPPED!";
        }

        if (ActiveScenario != null)
        {
            resultsBodyText.text = win ? ActiveScenario.successEnding : ActiveScenario.failureEnding;
        }
        else
        {
            resultsBodyText.text = "Communication ended.";
        }

        StartCoroutine(ShowResultsAfterDelay(2f));
    }

    IEnumerator ShowResultsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        resultsPanel.SetActive(true);
    }

    public void CloseResultsToPhone()
    {
        resultsPanel.SetActive(false);
        ActivePhonePanel.SetActive(false);

        if (IsCurrentLevelComplete())
        {
            ShowLevelComplete();
        }
        else
        {
            playerController.enabled = true;
        }
    }

    bool IsCurrentLevelComplete()
    {
        foreach (int idx in levels[currentLevelIndex].npcRosterIndices)
        {
            var status = npcRoster[idx].callStatus;
            if (status == CallStatus.Available) return false;
        }
        return true;
    }

    void ShowLevelComplete()
    {
        bool isFinalLevel = currentLevelIndex >= levels.Length - 1;

        if (levelCompletePanel != null)
        {
            levelCompleteTitleText.text = isFinalLevel
                ? "YOU WIN! ALL LEVELS COMPLETE!"
                : $"LEVEL {currentLevelIndex} COMPLETE!";
            levelCompleteBodyText.text = isFinalLevel
                ? "You've scammed everyone. Thanks for playing."
                : $"Moving on to Level {currentLevelIndex + 1}...";
            levelCompletePanel.SetActive(true);
        }

        playerController.enabled = true;
    }

    public void AdvanceToNextLevel()
    {

        PhoneInteractable[] allPhones = FindObjectsOfType<PhoneInteractable>();
        foreach (var p in allPhones)
        {
            p.ClosePanel();
        }

        if (currentLevelIndex >= levels.Length - 1) return;

        currentLevelIndex++;
        
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (playerSpawnPoint != null)
        {
            playerController.transform.position = playerSpawnPoint.position;
            playerController.transform.rotation = playerSpawnPoint.rotation;
        }

        playerController.enabled = true;
        //playerController.UnfreezePlayer();
    }

    public void ClosePhone()
    {
        ActivePhonePanel.SetActive(false);
        playerController.enabled = true;
    }

    public void OnPlayerMetNPC(int npcIndex)
    {
        // Unlock system disabled for now
    }

    public int GetNPCIndexByName(string name)
    {
        for (int i = 0; i < npcRoster.Count; i++)
            if (npcRoster[i].npcName == name) return i;
        Debug.LogWarning($"[GameManager] No NPC found with name: {name}");
        return -1;
    }
}