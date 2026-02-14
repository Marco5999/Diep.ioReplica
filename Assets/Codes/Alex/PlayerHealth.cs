using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Setup")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Custom Time-Based Auto-Regen")]
    public float regenDelay = 3f;              // Seconds before regen starts after damage
    public float regenPerSecond = 2f;          // Base HP regenerated per second
    public float regenRampTime = 1.5f;         // Smooth ramp-up time (0 = instant full speed)

    [Header("Invincibility")]
    public float invincibilityTime = 1f;       // Seconds invincible after hit

    private bool isInvincible = false;
    private float lastDamageTime;
    private float regenStartTime = -1f;        // When regen can begin
    private float regenRampProgress = 0f;      // 0-1 for ramp
    private float healthFloat;                 // Smooth health for fractional regen

    void Start()
    {
        currentHealth = maxHealth;
        healthFloat = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // Invincibility timer
        if (isInvincible && Time.time > lastDamageTime + invincibilityTime)
        {
            isInvincible = false;
        }

        // Custom Time-Based Regen
        if (currentHealth < maxHealth && Time.time >= regenStartTime)
        {
            // Ramp up regen speed
            regenRampProgress = Mathf.Clamp01((Time.time - regenStartTime) / regenRampTime);
            float currentRegenSpeed = regenPerSecond * regenRampProgress;

            // Apply smooth regen
            healthFloat += currentRegenSpeed * Time.deltaTime;
            currentHealth = Mathf.Min(maxHealth, Mathf.FloorToInt(healthFloat));

            UpdateHealthUI();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        healthFloat = currentHealth;  // sync float
        lastDamageTime = Time.time;
        isInvincible = true;

        // Reset regen timer on new damage
        regenStartTime = Time.time + regenDelay;

        Debug.Log("Player hit! HP: " + currentHealth + " | Regen starts in " + regenDelay + "s");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died! Restarting scene...");

        // Reload the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void UpdateHealthUI()
    {
        PlayerHealthBar healthBar = GetComponentInChildren<PlayerHealthBar>();
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }
}
