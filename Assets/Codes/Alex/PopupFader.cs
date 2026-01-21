using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PopupFader : MonoBehaviour
{
    [Header("UI Elements to Fade")]
    public Image backgroundImage;     // The panel's background Image
  
    public TextMeshProUGUI mainText;  // "LEVEL UP!"
    public TextMeshProUGUI subText;   // "Level X!"

    [Header("Timing")]
    public float fadeInTime = 0.6f;
    public float displayTime = 2.5f;
    public float fadeOutTime = 1f;

    // Store original colors (we'll modify alpha only)
    public Color bgOriginalColor;

    private Color mainTextOriginalColor;
    private Color subTextOriginalColor;

    void Awake()
    {
        // Cache original colors (including their starting alpha)
        if (backgroundImage) bgOriginalColor = backgroundImage.color;
        if (mainText) mainTextOriginalColor = mainText.color;
        if (subText) subTextOriginalColor = subText.color;

        // Start hidden
        SetAlpha(0f);
        gameObject.SetActive(false);
    }

    public void Show(string levelNumber)
    {
        // Update dynamic text
        if (mainText) mainText.text = "LEVEL UP!";
        if (subText) subText.text = "Level " + levelNumber + "!";

        // Reset to original colors but alpha 0
        SetAlpha(0f);

        gameObject.SetActive(true);

        // Start the fade sequence
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Fade IN
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeInTime;
            SetAlpha(Mathf.Lerp(0f, 1f, t));
            yield return null;
        }
        SetAlpha(1f);

        // Stay visible
        yield return new WaitForSeconds(displayTime);

        // Fade OUT
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeOutTime;
            SetAlpha(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        SetAlpha(0f);

        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        // Background
        if (backgroundImage)
        {
            Color c = bgOriginalColor;
            c.a = alpha;
            backgroundImage.color = c;
        }
        // Main text
        if (mainText)
        {
            Color c = mainTextOriginalColor;
            c.a = alpha;
            mainText.color = c;
        }

        // Sub text
        if (subText)
        {
            Color c = subTextOriginalColor;
            c.a = alpha;
            subText.color = c;
        }
    }
}