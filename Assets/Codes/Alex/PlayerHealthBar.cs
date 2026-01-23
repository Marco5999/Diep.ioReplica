using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI Refs")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Show on Damage")]
    public float showDuration = 2f;      // Show for 2s after damage
    public float fadeSpeed = 8f;         // Smooth fade in/out
    public float pulseSpeed = 4f;        // Pulse animation while shown

    private Canvas canvas;
    private float targetHealth = 100f;
    private float displayHealth = 100f;
    private bool isShown = false;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;  // Hidden start
        gameObject.SetActive(true);  // Active but invisible
    }

    void Update()
    {
        // SMOOTH FILL LERP (always)
        if (healthSlider)
        {
            displayHealth = Mathf.Lerp(displayHealth, targetHealth, fadeSpeed * Time.deltaTime);
            healthSlider.value = displayHealth;
        }
    }

    public void UpdateHealth(float current, float max)
    {
        targetHealth = current;
        healthSlider.maxValue = max;

        if (healthText)
        {
            healthText.text = Mathf.RoundToInt(current) + " / " + Mathf.RoundToInt(max);
        }

        // SHOW ON DAMAGE!
        if (!isShown)
        {
            ShowBar();
        }
        // Reset timer if damaged again
        StopAllCoroutines();
        StartCoroutine(HideAfterDuration());
    }

    private void ShowBar()
    {
        isShown = true;
        canvas.enabled = true;
        StartCoroutine(PulseAnimation());
    }

    private IEnumerator HideAfterDuration()
    {
        yield return new WaitForSeconds(showDuration);

        // FADE OUT
        float timer = 0f;
        float startAlpha = canvas.GetComponent<CanvasGroup>() ? canvas.GetComponent<CanvasGroup>().alpha : 1f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            if (canvas.GetComponent<CanvasGroup>())
                canvas.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, timer);
            yield return null;
        }

        canvas.enabled = false;
        isShown = false;
    }

    private IEnumerator PulseAnimation()
    {
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.gameObject.AddComponent<CanvasGroup>();

        while (isShown)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.1f + 0.9f;  // Subtle pulse
            cg.alpha = Mathf.Lerp(0.7f, 1f, pulse);
            yield return null;
        }
        cg.alpha = 1f;
    }
}