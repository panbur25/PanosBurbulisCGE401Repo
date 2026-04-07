using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CallData
{
    public string callerName;
    public string callerNumber;
    public string dialogue;
    public bool isScam;
    public string correctFeedback;
    public string wrongFeedback;
}

public class CallManager : MonoBehaviour
{
    [Header("Call Screen")]
    public Text callerNameText;
    public Text callerNumberText;
    public Text callerDialogueText;
    public GameObject callScreen;

    [Header("Feedback Screen")]
    public GameObject feedbackScreen;
    public Text feedbackText;
    public Text resultText;

    [Header("End Screen")]
    public GameObject endScreen;
    public Text finalScoreText;

    private int currentCallIndex = 0;
    private int score = 0;

    private CallData[] calls = new CallData[]
    {
        // CALL 1 — Obvious scam
        new CallData {
            callerName = "IRS DEPARTMENT",
            callerNumber = "+1 (800) 555-0192",
            dialogue = "This is Agent Davis from the IRS. You have unpaid taxes of $3,200. To avoid immediate arrest, you must pay right now using Google Play gift cards. Do you have a pen to write down the instructions?",
            isScam = true,
            correctFeedback = "Correct! This was a scam.\n\nThe IRS never calls demanding immediate payment, and no government agency ever accepts gift cards. When in doubt, hang up and call the agency directly using a number from their official website.",
            wrongFeedback = "This was a scam!\n\nThe IRS never calls demanding immediate payment, and no government agency ever accepts gift cards. Scammers use urgency and fear of arrest to pressure you into acting fast."
        },

        // CALL 2 — Legitimate call
        new CallData {
            callerName = "FIRST NATIONAL BANK",
            callerNumber = "+1 (312) 555-0847",
            dialogue = "Hi, this is Sarah from First National Bank's fraud department. We've noticed an unusual charge of $847 on your account from an overseas retailer. I just need to confirm, did you make this purchase? If not, we can freeze the card. I'm not asking for any account details, just a yes or no.",
            isScam = false,
            correctFeedback = "Correct! This was a legitimate call.\n\nKey signs it was real: they didn't ask for passwords or full account numbers, they gave you a specific transaction to verify, and they weren't pressuring you to act urgently. When a bank calls about fraud, it's okay to engage, but you can always hang up and call the number on the back of your card to verify.",
            wrongFeedback = "This was actually a legitimate fraud alert from a bank!\n\nNot every suspicious-sounding call is a scam. Real bank fraud departments call customers about unusual charges. The tell here was that they asked only for a yes or no: no passwords, no full card numbers, no gift cards. Hanging up on everything can cause you to miss real alerts."
        },

        // CALL 3 — Subtle scam
        new CallData {
            callerName = "SOCIAL SECURITY ADMIN",
            callerNumber = "+1 (866) 555-0033",
            dialogue = "Good afternoon. This is Officer Mitchell with the Social Security Administration. Your Social Security number has been suspended due to suspicious activity linked to a vehicle found in Texas. To reactivate it and avoid a federal warrant, I'll need you to verify your identity by confirming your SSN and date of birth.",
            isScam = true,
            correctFeedback = "Correct! This was a scam.\n\nThis one is trickier. It sounds official and uses real-sounding language. The red flags: the SSA never 'suspends' Social Security numbers, they never threaten federal warrants over the phone, and they will never ask you to verify your SSN by reading it back to them over an unsolicited call.",
            wrongFeedback = "This was a scam, and a clever one!\n\nIt sounds official, but the SSA never suspends Social Security numbers or threatens warrants by phone. The biggest red flag: they asked you to read your SSN back to them. A real government agency already has your information, they would never ask you to confirm it over an unsolicited call."
        }
    };

    void Start()
    {
        currentCallIndex = 0;
        score = 0;
        LoadCall(currentCallIndex);
    }

    void LoadCall(int index)
    {
        callScreen.SetActive(true);
        feedbackScreen.SetActive(false);
        endScreen.SetActive(false);

        CallData call = calls[index];
        callerNameText.text = call.callerName;
        callerNumberText.text = call.callerNumber;
        callerDialogueText.text = call.dialogue;
    }

    public void OnPlayerHungUp()
    {
        EvaluateChoice(true);
    }

    public void OnPlayerComplied()
    {
        EvaluateChoice(false);
    }

    private void EvaluateChoice(bool playerHungUp)
    {
        CallData call = calls[currentCallIndex];
        bool correctChoice = (playerHungUp == call.isScam);

        if (correctChoice) score++;

        callScreen.SetActive(false);
        feedbackScreen.SetActive(true);

        resultText.text = correctChoice ? "CORRECT" : "WRONG";
        feedbackText.text = correctChoice ? call.correctFeedback : call.wrongFeedback;
    }

    public void OnNextCall()
    {
        currentCallIndex++;

        if (currentCallIndex >= calls.Length)
        {
            feedbackScreen.SetActive(false);
            endScreen.SetActive(true);
            finalScoreText.text = "You got " + score + " out of " + calls.Length + " calls right.";
        }
        else
        {
            LoadCall(currentCallIndex);
        }
    }
}