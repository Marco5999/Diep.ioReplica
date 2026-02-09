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
    public float attackSpeedPerUpgrade = 0.2f;
    public float regenerationPerUpgrade = 5f;
    public int playerHpPerUpgrade = 20;

    [Header("Popup UI")]
    public GameObject upgradePopupPrefab;
    public Canvas upgradeCanvas;

    // Cached player components – updated every frame if null
    private PlayerMoveAndCamera moveScript;
    private PlayerShooting shootScript;
    private PlayerHealth healthScript;

    private GameObject currentPopup;

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

    void Update()
    {
        // Safe late caching – in case player spawns late or scene reloads
        if (moveScript == null || shootScript == null || healthScript == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                moveScript   = player.GetComponent<PlayerMoveAndCamera>();
                shootScript  = player.GetComponent<PlayerShooting>();
                healthScript = player.GetComponent<PlayerHealth>();

                if (moveScript == null)   Debug.LogWarning("[UpgradeManager] PlayerMoveAndCamera missing on Player!");
                if (shootScript == null)  Debug.LogWarning("[UpgradeManager] PlayerShooting missing on Player!");
                if (healthScript == null) Debug.LogWarning("[UpgradeManager] PlayerHealth missing on Player!");
            }
        }
    }

    public void GainUpgradePoint()
    {
        upgradePoints++;
        Debug.Log($"[Upgrade] Gained point → now {upgradePoints}");

        if (upgradePoints > 0)
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
            case UpgradeType.Damage:
                damageLevel++;
                Debug.Log($"[Upgrade] Damage → Lv.{damageLevel}");
                break;

            case UpgradeType.PlayerSpeed:
                playerSpeedLevel++;
                if (moveScript != null)
                {
                    moveScript.moveSpeed += playerSpeedPerUpgrade;
                    Debug.Log($"[Upgrade] Speed → {moveScript.moveSpeed}");
                }
                else Debug.LogError("[Upgrade] Cannot apply PlayerSpeed – moveScript is null");
                break;

            case UpgradeType.AttackSpeed:
                attackSpeedLevel++;
                if (shootScript != null)
                {
                    shootScript.fireRate += attackSpeedPerUpgrade;
                    Debug.Log($"[Upgrade] Attack Speed → {shootScript.fireRate}");
                }
                else Debug.LogError("[Upgrade] Cannot apply AttackSpeed – shootScript is null");
                break;

            case UpgradeType.Regeneration:
                regenerationLevel++;
                if (healthScript != null)
                {
                    healthScript.regenPerSecond += regenerationPerUpgrade;
                    Debug.Log($"[Upgrade] Regen → {healthScript.regenPerSecond}/sec");
                }
                else Debug.LogError("[Upgrade] Cannot apply Regeneration – healthScript is null");
                break;

            case UpgradeType.PlayerHp:
                playerHpLevel++;
                if (healthScript != null)
                {
                    healthScript.maxHealth += playerHpPerUpgrade;
                    healthScript.currentHealth = healthScript.maxHealth; // heal to new max
                    // Force UI refresh
                    healthScript.UpdateHealthUI();
                    Debug.Log($"[Upgrade] HP → max {healthScript.maxHealth}, current {healthScript.currentHealth}");
                }
                else Debug.LogError("[Upgrade] Cannot apply PlayerHp – healthScript is null");
                break;
        }

        // Refresh popup UI if open
        if (currentPopup != null)
        {
            var ui = currentPopup.GetComponent<UpgradeUI>();
            if (ui != null) ui.RefreshUI();
        }

        if (upgradePoints <= 0 && currentPopup != null)
        {
            HidePopup();
        }
    }

    public int GetLevel(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Damage        => damageLevel,
            UpgradeType.PlayerSpeed   => playerSpeedLevel,
            UpgradeType.AttackSpeed   => attackSpeedLevel,
            UpgradeType.Regeneration  => regenerationLevel,
            UpgradeType.PlayerHp      => playerHpLevel,
            _                         => 0
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

        if (currentPopup != null) Destroy(currentPopup);

        currentPopup = Instantiate(upgradePopupPrefab, upgradeCanvas.transform, false);
        var ui = currentPopup.GetComponent<UpgradeUI>();
        if (ui != null) ui.Show();
    }

    public void HidePopup()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
            currentPopup = null;
        }
    }
}