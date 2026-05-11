using UnityEngine;
using UnityEngine.UI;

public class ContactButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text statusText;
    [SerializeField] private Text descriptText;
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image profilePicImage;

    [Header("Status Colors")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);   // slight gray, mostly transparent
    [SerializeField] private Color availableColor = new Color(1f, 1f, 1f, 1f);         // fully visible/normal
    [SerializeField] private Color scammedColor = new Color(0.2f, 0.8f, 0.2f, 0.6f);  // green tint, visible
    [SerializeField] private Color hungUpColor = new Color(0.8f, 0.2f, 0.2f, 0.6f);   // red tint, visible

    private int npcIndex;

    public void Setup(int index, string npcName, CallStatus status, Sprite profilePic, string description)
    {
        npcIndex = index;

        /*
        if (profilePicImage != null)
        {
            profilePicImage.sprite = (profilePic != null) ? profilePic : defaultProfilePic;
        } */

        if (descriptText != null) descriptText.text = description;
        ApplyStatus(status, npcName, description);
    }

    private void ApplyStatus(CallStatus status, string npcName = "Unknown", string description = "")
    {
        switch (status)
        {
            case CallStatus.Locked:
                if (nameText != null) nameText.text = "Unknown";
                if (statusText != null) statusText.text = "???";
                if (descriptText != null) descriptText.text = "???";
                if (backgroundImage != null) backgroundImage.color = lockedColor;
                if (button != null) button.interactable = false;
                if (profilePicImage != null) profilePicImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                break;
            case CallStatus.Available:
                if (nameText != null) nameText.text = npcName;
                if (statusText != null) statusText.text = "Available";
                if (descriptText != null) descriptText.text = description;
                if (backgroundImage != null) backgroundImage.color = availableColor;
                if (button != null) button.interactable = true;
                if (profilePicImage != null) profilePicImage.color = Color.white;
                break;
            case CallStatus.Scammed:
                if (nameText != null) nameText.text = npcName;
                if (statusText != null) statusText.text = "Scammed";
                if (descriptText != null) descriptText.text = description;
                if (backgroundImage != null) backgroundImage.color = scammedColor;
                if (button != null) button.interactable = false;
                if (profilePicImage != null) profilePicImage.color = Color.white;
                break;
            case CallStatus.HungUp:
                if (nameText != null) nameText.text = npcName;
                if (statusText != null) statusText.text = "Hung Up";
                if (descriptText != null) descriptText.text = description;
                if (backgroundImage != null) backgroundImage.color = hungUpColor;
                if (button != null) button.interactable = false;
                if (profilePicImage != null) profilePicImage.color = Color.white;
                break;
        }
    }

    // Call this from PhoneUI whenever the phone panel is opened,
    // so buttons react to world progress without a full rebuild.
    public void RefreshStatus()
    {
        CallStatus current = GameManager.Instance.GetCallStatus(npcIndex);
        NPCEntry npc = GameManager.Instance.NPCRoster[npcIndex];
        if (profilePicImage != null)
            profilePicImage.sprite = npc.profilePic;
        ApplyStatus(current, npc.npcName, npc.contactDescription);
    }

    public void OnClick()
    {
        GameManager.Instance.StartGameWithNPC(npcIndex);
    }
}