using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointTracker : MonoBehaviour
{
    public static PointTracker Instance;

    [Header("UI References")]
    public Slider pointSlider;
    public TextMeshProUGUI levelText;

    [Header("Dynamic Color Gradient")]
    public Gradient fillGradient;  // Assign in Inspector: Red(0) → Yellow(0.5) → Green(1)

    [Header("Animation Settings")]
    public float sliderSpeed = 50f;  // Points per second

    private Image fillImage;       // Cache for fill color changes
    private float currentPoints = 0f;
    private float targetPoints = 0f;  // Target value for smooth animation
    private float currentMaxPoints = 100f;
    private int currentLevel = 1;

    [Header("Popup")]
    public GameObject levelUpPopupPrefab;
    private PopupFader currentPopup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pointSlider == null) Debug.LogError("Point Slider not assigned!");
        if (levelText == null) Debug.LogError("Level Text not assigned!");
        if (fillGradient == null) Debug.LogError("Fill Gradient not assigned!");

        // Cache fill image
        fillImage = pointSlider.fillRect.GetComponent<Image>();
        if (fillImage == null) Debug.LogError("Slider Fill Image not found!");

        // Init
        pointSlider.minValue = 0f;
        pointSlider.maxValue = currentMaxPoints;
        pointSlider.value = currentPoints;
        targetPoints = currentPoints;
        levelText.text = "Lvl " + currentLevel;
        UpdateFillColor();  // Start empty color
    }

    private void Update()
    {
        // Smoothly animate slider towards target
        if (currentPoints != targetPoints)
        {
            float moveAmount = sliderSpeed * Time.deltaTime;
            currentPoints = Mathf.MoveTowards(currentPoints, targetPoints, moveAmount);
            pointSlider.value = currentPoints;
            UpdateFillColor();  // Update color during animation

            // Check for level up when reaching target
            if (currentPoints >= currentMaxPoints && targetPoints >= currentMaxPoints)
            {
                LevelUp();
            }
        }
    }

    public void UpdatePointFill(int pointsToAdd)
    {
        targetPoints += pointsToAdd;
        targetPoints = Mathf.Clamp(targetPoints, 0f, currentMaxPoints);
        // Slider will smoothly animate to targetPoints in Update()
    }

    private void UpdateFillColor()
    {
        float progress = currentPoints / currentMaxPoints;
        fillImage.color = fillGradient.Evaluate(progress);  // SMOOTH COLOR BLEND!
    }

    private void LevelUp()
    {
        currentPoints = 0f;
        targetPoints = 0f;
        currentLevel++;
        currentMaxPoints *= 1.5f;

        pointSlider.maxValue = currentMaxPoints;
        pointSlider.value = currentPoints;

        levelText.text = "Lvl " + currentLevel;
        UpdateFillColor();  // Reset to start color (red/empty)

        Debug.Log("LEVEL UP! Level " + currentLevel + " (Max: " + currentMaxPoints + ")");

        // Popup (unchanged)
        if (levelUpPopupPrefab != null)
        {
            if (currentPopup != null) Destroy(currentPopup.gameObject);
            GameObject popupObj = Instantiate(levelUpPopupPrefab, Vector3.zero, Quaternion.identity);
            popupObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
            currentPopup = popupObj.GetComponent<PopupFader>();
            if (currentPopup != null)
            {
                currentPopup.Show(currentLevel.ToString());
            }
        }
    }
}