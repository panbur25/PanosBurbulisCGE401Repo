using UnityEngine;
using UnityEngine.UI;

public class ContactButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text statusText;
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;

    [Header("Status Colors")]
    [SerializeField] private Color lockedColor = new Color(0.08f, 0.08f, 0.08f); // NEW — very dark
    [SerializeField] private Color availableColor = new Color(0.15f, 0.15f, 0.2f);
    [SerializeField] private Color scammedColor = new Color(0.1f, 0.35f, 0.1f);
    [SerializeField] private Color hungUpColor = new Color(0.35f, 0.1f, 0.1f);

    private int npcIndex;

    public void Setup(int index, string npcName, CallStatus status)
    {
        npcIndex = index;
        if (nameText != null)
            nameText.text = npcName;

        ApplyStatus(status);
    }

    // Call this from PhoneUI whenever the phone panel is opened,
    // so buttons react to world progress without a full rebuild.
    public void RefreshStatus()
    {
        CallStatus current = GameManager.Instance.GetCallStatus(npcIndex);  // CallStatus used directly, not GameManager.CallStatus
        ApplyStatus(current);

        if (current != CallStatus.Locked && nameText != null)
            nameText.text = GameManager.Instance.NPCRoster[npcIndex].npcName;
    }

    private void ApplyStatus(CallStatus status)
    {
        switch (status)
        {
            case CallStatus.Locked:
                if (statusText != null) statusText.text = "???";
                if (backgroundImage != null) backgroundImage.color = lockedColor;
                if (button != null) button.interactable = false;
                if (nameText != null) nameText.text = "Unknown"; // hide until met
                break;

            case CallStatus.Available:
                if (statusText != null) statusText.text = "Available";
                if (backgroundImage != null) backgroundImage.color = availableColor;
                if (button != null) button.interactable = true;
                break;

            case CallStatus.Scammed:
                if (statusText != null) statusText.text = "Scammed";
                if (backgroundImage != null) backgroundImage.color = scammedColor;
                if (button != null) button.interactable = false;
                break;

            case CallStatus.HungUp:
                if (statusText != null) statusText.text = "Hung Up";
                if (backgroundImage != null) backgroundImage.color = hungUpColor;
                if (button != null) button.interactable = false;
                break;
        }
    }

    public void OnClick()
    {
        GameManager.Instance.StartGameWithNPC(npcIndex);
    }
}