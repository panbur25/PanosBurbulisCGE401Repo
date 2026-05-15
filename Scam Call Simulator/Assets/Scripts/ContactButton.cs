using UnityEngine;
using UnityEngine.UI;
using TMPro; // Added for TextMeshPro support

public class ContactButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;   // Changed to TMP
    [SerializeField] private TextMeshProUGUI statusText; // Changed to TMP
    [SerializeField] private TextMeshProUGUI descriptText; // Changed to TMP
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image profilePicImage;

    [Header("Status Colors")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    [SerializeField] private Color availableColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color scammedColor = new Color(0.2f, 0.8f, 0.2f, 0.6f);
    [SerializeField] private Color hungUpColor = new Color(0.8f, 0.2f, 0.2f, 0.6f);

    private int npcIndex;

    public void Setup(int index, string npcName, CallStatus status, Sprite profilePic, string description)
    {
        npcIndex = index;

        // Set the profile picture
        if (profilePicImage != null && profilePic != null)
        {
            profilePicImage.sprite = profilePic;
        }

        ApplyStatus(status, npcName, description);

        // Auto-assign the button click in code so you don't have to do it manually in the prefab
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void ApplyStatus(CallStatus status, string npcName = "Unknown", string description = "")
    {
        // Visual logic based on status
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

    public void OnClick()
    {
        GameManager.Instance.StartGameWithNPC(npcIndex);
    }
}