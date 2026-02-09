using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Setup")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Custom Time-Based Auto-Regen")]
    public float regenDelay = 3f;              // Seconds before regen starts after damage
    public float regenPerSecond = 2f;          // HP regenerated per second (control speed!)
    public float regenRampTime = 1.5f;         // Smooth ramp-up time (0 = instant full speed)

    [Header("Invincibility")]
    public float invincibilityTime = 1f;       // Seconds invincible after hit

    private bool isInvincible = false;
    private float lastDamageTime;
    private float regenStartTime = -1f;        // When regen can begin
    private float regenRampProgress = 0f;      // 0-1 for ramp

    void Start()
    {
        currentHealth = maxHealth;
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
        if (currentHealth < maxHealth && !isInvincible)
        {
            // Check if delay passed
            if (Time.time >= regenStartTime)
            {
                // Ramp up regen speed
                regenRampProgress = Mathf.Clamp01((Time.time - regenStartTime) / regenRampTime);
                float currentRegenSpeed = regenPerSecond * regenRampProgress;

                // Regen this frame (time-based!)
                currentHealth += (int)(currentRegenSpeed * Time.deltaTime);
                currentHealth = Mathf.Min(currentHealth, maxHealth);

                UpdateHealthUI();  // Smooth bar update
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;
        isInvincible = true;

        // Reset regen timer on new damage
        regenStartTime = Time.time + regenDelay;

        Debug.Log("Player hit! HP: " + currentHealth + " | Regen starts in " + regenDelay + "s");

        UpdateHealthUI();  // Show bar!

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        transform.position = Vector3.zero;
        currentHealth = maxHealth;
        regenStartTime = -1f;  // Reset regen
        UpdateHealthUI();
        isInvincible = true;   // Brief god mode
        Debug.Log("Player Died! Respawned with full HP.");
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