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

    private Image fillImage;       // Cache for fill color changes
    private float currentPoints = 0f;
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
        levelText.text = "Level " + currentLevel;
        UpdateFillColor();  // Start empty color
    }

    public void UpdatePointFill(int pointsToAdd)
    {
        currentPoints += pointsToAdd;
        currentPoints = Mathf.Clamp(currentPoints, 0f, currentMaxPoints);
        pointSlider.value = currentPoints;

        UpdateFillColor();  // COLOR SHIFT!

        if (currentPoints >= currentMaxPoints)
        {
            LevelUp();
        }
    }

    private void UpdateFillColor()
    {
        float progress = currentPoints / currentMaxPoints;
        fillImage.color = fillGradient.Evaluate(progress);  // SMOOTH COLOR BLEND!
    }

    private void LevelUp()
    {
        currentPoints = 0f;
        currentLevel++;
        currentMaxPoints *= 1.5f;

        pointSlider.maxValue = currentMaxPoints;
        pointSlider.value = currentPoints;

        levelText.text = "Level " + currentLevel;
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