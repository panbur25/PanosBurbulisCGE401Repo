using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrustBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI trustLabel; 
    public TextMeshProUGUI deltaLabel; 
    public Image fillImage;
    public RectTransform fillRect; 
    public float maxFillWidth = 400f;

    private float targetTrust = 50f; // The value we WANT to be at
    private float visualTrust = 50f; // The value currently SHOWN on the bar
    public float TrustValue => targetTrust;

    [Header("Settings")]
    public float lerpSpeed = 5f; // How fast the bar moves

    private Coroutine deltaCoroutine;

    private void Update()
    {
        // Smoothly move the visual value toward the target value
        if (!Mathf.Approximately(visualTrust, targetTrust))
        {
            visualTrust = Mathf.MoveTowards(visualTrust, targetTrust, lerpSpeed * Time.deltaTime * 20f);
            UpdateUIElements(visualTrust);
        }
    }

    public void SetTrust(float value)
    {
        targetTrust = Mathf.Clamp(value, 0f, 100f);
        // If you want it to snap instantly at the very start of a level:
        // visualTrust = targetTrust; 
    }

    private void UpdateUIElements(float valueToDisplay)
    {
        slider.value = valueToDisplay / 100f;

        // --- CENTER-OUT MATH ---
        if (fillRect != null)
        {
            float distanceFromCenter = Mathf.Abs(valueToDisplay - 50f);
            float newWidth = (distanceFromCenter / 50f) * (maxFillWidth / 2f);
            fillRect.sizeDelta = new Vector2(newWidth, fillRect.sizeDelta.y);
        }

        // --- OPTION A: SCENARIO LOGIC ---
        UpdateLabelAndColors(valueToDisplay);
    }

    private void UpdateLabelAndColors(float val)
    {
        bool isScammer = GameManager.Instance.CurrentScenario == NPCScenario.Scammer;
        float t = val / 100f;

        if (isScammer)
        {
            // SCAMMER MODE: High Trust is GOOD (Green)
            trustLabel.text = "Trust: " + Mathf.RoundToInt(val);
            fillImage.color = GetColor(t, Color.red, Color.white, Color.green);
        }
        else
        {
            // VICTIM MODE: High Suspicion is BAD (Red)
            // We flip the colors so 100% Suspicion = Bright Red
            trustLabel.text = "Suspicion: " + Mathf.RoundToInt(val);
            fillImage.color = GetColor(t, Color.green, Color.white, Color.red);
        }
    }

    private Color GetColor(float t, Color lowColor, Color midColor, Color highColor)
    {
        if (t < 0.5f)
            return Color.Lerp(lowColor, midColor, Mathf.Pow(t * 2f, 3f));
        else
            return Color.Lerp(midColor, highColor, Mathf.Pow((t - 0.5f) * 2f, 0.33f));
    }

    private void ShowDelta(float delta)
    {
        if (deltaLabel == null) return;

        if (deltaCoroutine != null)
            StopCoroutine(deltaCoroutine);

        deltaCoroutine = StartCoroutine(FadeDelta(delta));
    }

    public void ModifyTrust(float delta)
    {
        SetTrust(targetTrust + delta);
        ShowDelta(delta); // This ensures the +15 or -10 appears on screen!
    }

    public void ResetDelta()
    {
        if (deltaCoroutine != null)
        {
            StopCoroutine(deltaCoroutine);
            deltaCoroutine = null;
        }
        if (deltaLabel != null)
        {
            deltaLabel.text = "";
            // Resetting alpha to 1f
            deltaLabel.alpha = 1f;
        }
    }

    private IEnumerator FadeDelta(float delta)
    {
        deltaLabel.text = (delta >= 0 ? "+" : "") + Mathf.RoundToInt(delta).ToString();
        deltaLabel.color = delta >= 0 ? Color.green : Color.red;
        deltaLabel.alpha = 1f;

        yield return new WaitForSeconds(2f);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            // TMP has a direct .alpha property which is cleaner than making a new Color
            deltaLabel.alpha = 1f - elapsed;
            yield return null;
        }

        deltaLabel.text = "";
        deltaLabel.alpha = 1f;
    }
}