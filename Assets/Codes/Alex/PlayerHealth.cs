using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Setup")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Regen & Invincibility")]
    public float regenRate = 1f;           // HP per second
    public float invincibilityTime = 1f;   // Frames after hit

    private bool isInvincible = false;
    private float lastDamageTime;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // Slow regen
        if (!isInvincible && currentHealth < maxHealth)
        {
            currentHealth += (int)(regenRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            UpdateHealthUI();
        }

        // Invincibility timer
        if (isInvincible && Time.time > lastDamageTime + invincibilityTime)
        {
            isInvincible = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;
        isInvincible = true;

        Debug.Log("Player hit! HP: " + currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        transform.position = Vector3.zero;
        currentHealth = maxHealth;
        UpdateHealthUI();
        isInvincible = true;
        Debug.Log("Player Died! Respawned.");
    }

    private void UpdateHealthUI()
    {
        PlayerHealthBar healthBar = GetComponentInChildren<PlayerHealthBar>();
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }
}