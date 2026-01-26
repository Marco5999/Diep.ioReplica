using UnityEngine;

public enum UpgradeType { Damage, PlayerSpeed, AttackSpeed, Regeneration, PlayerHp }

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Upgrade Points")]
    public int upgradePoints = 0;

    [Header("Upgrade Levels")]
    public int damageLevel = 0;
    public int playerSpeedLevel = 0;
    public int attackSpeedLevel = 0;
    public int regenerationLevel = 0;
    public int playerHpLevel = 0;

    [Header("Per-Upgrade Bonuses")]
    public int damagePerUpgrade = 1;
    public float playerSpeedPerUpgrade = 1f;
    public float attackSpeedPerUpgrade = 2f;
    public float regenerationPerUpgrade = 0.5f;
    public int playerHpPerUpgrade = 20;

    [Header("Popup UI")]
    public GameObject upgradePopupPrefab;
    public Canvas upgradeCanvas;

    private GameObject currentPopup;
    private UpgradeUI currentUI;  // Cache the UI reference for live refresh

    void Awake()
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

    public void GainUpgradePoint()
    {
        upgradePoints++;
        Debug.Log($"[Upgrade] Gained point → now {upgradePoints}");

        // If popup already open → just refresh it
        if (currentPopup != null && currentUI != null)
        {
            currentUI.RefreshUI();
        }
        // Otherwise open fresh popup
        else if (upgradePoints > 0)
        {
            ShowPopup();
        }
    }

    public void Upgrade(UpgradeType type)
    {
        if (upgradePoints <= 0) return;

        upgradePoints--;

        switch (type)
        {
            case UpgradeType.Damage:        damageLevel++; break;
            case UpgradeType.PlayerSpeed:   playerSpeedLevel++; break;
            case UpgradeType.AttackSpeed:   attackSpeedLevel++; break;
            case UpgradeType.Regeneration:  regenerationLevel++; break;
            case UpgradeType.PlayerHp:      playerHpLevel++; break;
        }

        Debug.Log($"[Upgrade] Upgraded {type} → Lv.{GetLevel(type)} | Points left: {upgradePoints}");

        // Refresh UI immediately after upgrade
        if (currentUI != null)
        {
            currentUI.RefreshUI();
        }

        // Auto-close when no points left
        if (upgradePoints <= 0 && currentPopup != null)
        {
            HidePopup();
        }
    }

    public int GetLevel(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Damage => damageLevel,
            UpgradeType.PlayerSpeed => playerSpeedLevel,
            UpgradeType.AttackSpeed => attackSpeedLevel,
            UpgradeType.Regeneration => regenerationLevel,
            UpgradeType.PlayerHp => playerHpLevel,
            _ => 0
        };
    }

    public int GetBulletDamage() => 1 + (damageLevel * damagePerUpgrade);

    private void ShowPopup()
    {
        if (upgradeCanvas == null || upgradePopupPrefab == null)
        {
            Debug.LogError("[UpgradeManager] Canvas or prefab missing!");
            return;
        }

        if (currentPopup != null)
        {
            Destroy(currentPopup);
        }

        currentPopup = Instantiate(upgradePopupPrefab, upgradeCanvas.transform, false);
        currentUI = currentPopup.GetComponent<UpgradeUI>();

        if (currentUI == null)
        {
            Debug.LogError("[UpgradeManager] No UpgradeUI on popup prefab!");
            return;
        }

        currentUI.Show();
    }

    public void HidePopup()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
            currentPopup = null;
            currentUI = null;
        }
    }
}