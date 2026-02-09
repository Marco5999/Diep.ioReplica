using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI Refs")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Position Offset (Below Player)")]
    public Vector3 positionOffset = new Vector3(0f, -1.2f, 0f);  // Tweak Y for perfect spot

    [Header("Show on Damage")]
    public float showDuration = 2f;
    public float fadeSpeed = 8f;
    public float pulseSpeed = 4f;

    private Canvas canvas;
    private Transform playerTransform;  // Reference to parent Player
    private float targetHealth = 100f;
    private float displayHealth = 100f;
    private bool isShown = false;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;  // Hidden by default
    }

    void Start()
    {
        // Cache Player transform (parent)
        playerTransform = transform.parent;
        if (playerTransform == null)
        {
            Debug.LogError("HealthBarCanvas must be child of Player!");
        }
    }

    void LateUpdate()
    {
        // FOLLOW PLAYER POSITION ONLY (no rotation!)
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + positionOffset;
        }

        // FORCE NO ROTATION – always upright!
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // SMOOTH FILL LERP
        if (healthSlider != null)
        {
            displayHealth = Mathf.Lerp(displayHealth, targetHealth, fadeSpeed * Time.deltaTime);
            healthSlider.value = displayHealth;
        }
    }

    public void UpdateHealth(float current, float max)
    {
        targetHealth = current;
        if (healthSlider != null) healthSlider.maxValue = max;

        if (healthText != null)
        {
            healthText.text = Mathf.RoundToInt(current) + " / " + Mathf.RoundToInt(max);
        }

        // SHOW BAR ON DAMAGE
        if (!isShown)
        {
            ShowBar();
        }
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

        // FADE OUT SMOOTH
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.gameObject.AddComponent<CanvasGroup>();

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            cg.alpha = Mathf.Lerp(1f, 0f, timer);
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
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.1f + 0.9f;
            cg.alpha = Mathf.Lerp(0.7f, 1f, pulse);
            yield return null;
        }
        cg.alpha = 1f;
    }
}