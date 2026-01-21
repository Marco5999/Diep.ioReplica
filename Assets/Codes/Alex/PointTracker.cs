using UnityEngine;
using UnityEngine.UI;
using System.Collections;  // For coroutine if needed
using TMPro;

public class PointTracker : MonoBehaviour
{
    public static PointTracker Instance;  // Global access
    [Header("Level Up Popup")]
    public GameObject levelUpPopupPrefab;  // Drag the prefab here!
    private PopupFader currentPopup;       // To avoid multiple at once

    [Header("UI References")]
    public Slider pointSlider;           // NEW: Drag your PointSlider here!
    public TextMeshProUGUI levelText;    // Still your Level text

    public float currentPoints = 0f;
    public float currentMaxPoints = 100f;  // Renamed for clarity
    public int currentLevel = 1;

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
        if (pointSlider == null)
        {
            Debug.LogError("Point Slider is not assigned!");
        }
        if (levelText == null)
        {
            Debug.LogError("Level Text is not assigned!");
        }

        // Initialize UI
        pointSlider.minValue = 0f;
        pointSlider.maxValue = currentMaxPoints;
        pointSlider.value = currentPoints;
        levelText.text = "Level " + currentLevel;
    }

    public void UpdatePointFill(int pointsToAdd)
    {
        currentPoints += pointsToAdd;

        // Clamp to current max
        currentPoints = Mathf.Clamp(currentPoints, 0f, currentMaxPoints);

        // Update slider visually
        pointSlider.value = currentPoints;

        if (currentPoints >= currentMaxPoints)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentPoints = 0f;
        currentLevel++;
        currentMaxPoints *= 1.5f;  // Next level requires more points

        // Update slider range & reset
        pointSlider.maxValue = currentMaxPoints;
        pointSlider.value = currentPoints;

        levelText.text = "Level " + currentLevel;
        if (levelUpPopupPrefab != null)
{
    // Destroy old one if exists (safety)
    if (currentPopup != null)
    {
        Destroy(currentPopup.gameObject);
    }

    // Spawn new popup
    GameObject popupObj = Instantiate(levelUpPopupPrefab, Vector3.zero, Quaternion.identity);
    popupObj.transform.SetParent(GameObject.Find("Canvas").transform, false);  // Attach to main Canvas
    currentPopup = popupObj.GetComponent<PopupFader>();

    if (currentPopup != null)
    {
        currentPopup.Show(currentLevel.ToString());
    }
    }

        Debug.Log("LEVEL UP! Now Level " + currentLevel + " (Max: " + currentMaxPoints + ")");
    }
}