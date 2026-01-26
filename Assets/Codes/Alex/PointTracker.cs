using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointTracker : MonoBehaviour
{
    public static PointTracker Instance;

    [Header("UI References")]
    public Slider pointSlider;
    public TextMeshProUGUI levelText;

    [Header("Slider Animation")]
    public float sliderSmoothSpeed = 5f;

    private float currentPoints = 0f;
    private float currentMaxPoints = 100f;
    private int currentLevel = 1;

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

        pointSlider.minValue = 0f;
        pointSlider.maxValue = currentMaxPoints;
        pointSlider.value = currentPoints;
        levelText.text = "Level " + currentLevel;
    }

    private void Update()
    {
        // Smoothly move slider toward currentPoints every frame
        if (pointSlider != null)
        {
            pointSlider.value = Mathf.Lerp(pointSlider.value, currentPoints, sliderSmoothSpeed * Time.deltaTime);
        }
    }

    public void UpdatePointFill(int pointsToAdd)
    {
        currentPoints += pointsToAdd;
        currentPoints = Mathf.Clamp(currentPoints, 0f, currentMaxPoints);

        if (currentPoints >= currentMaxPoints)
        {
            LevelUp();
        }
    }

 private void LevelUp()
{
    currentPoints = 0f;
    currentLevel++;
    currentMaxPoints *= 1.5f;
    pointSlider.maxValue = currentMaxPoints;
    levelText.text = "Level " + currentLevel;

    // This line MUST exist
    if (UpgradeManager.Instance != null)
    {
        UpgradeManager.Instance.GainUpgradePoint();
    }

    Debug.Log($"LEVEL UP to {currentLevel} – called GainUpgradePoint");
}
}