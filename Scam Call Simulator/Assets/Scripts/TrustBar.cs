using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrustBar : MonoBehaviour
{
    public Slider slider;
    public Text trustLabel;
    public Text deltaLabel;  // drag a new Text object here for the delta
    public Image fillImage;  // drag the Fill image (inside Fill Area) here

    private float trustValue = 50f;
    public float TrustValue => trustValue;

    private Coroutine deltaCoroutine;

    public void SetTrust(float value)
    {
        trustValue = Mathf.Clamp(value, 0f, 100f);
        slider.value = trustValue / 100f;
        trustLabel.text = "Trust: " + Mathf.RoundToInt(trustValue);

        if (fillImage != null)
        {
            float t = trustValue / 100f;
            Color fillColor;

            if (t < 0.5f)
                fillColor = Color.Lerp(Color.red, Color.white, Mathf.Pow(t * 2f, 3f));
            else
                fillColor = Color.Lerp(Color.white, Color.green, Mathf.Pow((t - 0.5f) * 2f, 0.33f));

            fillImage.color = fillColor;
        }
    }

    public void ModifyTrust(float delta)
    {
        SetTrust(trustValue + delta);
        ShowDelta(delta);
    }

    private void ShowDelta(float delta)
    {
        if (deltaLabel == null) return;

        // Cancel any existing fade so they don't stack
        if (deltaCoroutine != null)
            StopCoroutine(deltaCoroutine);

        deltaCoroutine = StartCoroutine(FadeDelta(delta));
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
            deltaLabel.color = new Color(deltaLabel.color.r, deltaLabel.color.g, deltaLabel.color.b, 1f);
        }
    }

    private IEnumerator FadeDelta(float delta)
    {
        deltaLabel.text = (delta >= 0 ? "+" : "") + Mathf.RoundToInt(delta).ToString();
        deltaLabel.color = delta >= 0 ? Color.green : Color.red;

        // Hold for 2 seconds then fade out over 1 second
        yield return new WaitForSeconds(2f);

        float elapsed = 0f;
        Color startColor = deltaLabel.color;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            deltaLabel.color = new Color(startColor.r, startColor.g, startColor.b, 1f - elapsed);
            yield return null;
        }

        deltaLabel.text = "";
        deltaLabel.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }
}