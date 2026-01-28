using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [HideInInspector] public EnemySpawner spawner;

    [Header("Health Settings")]
    public float baseMaxHealth = 3f;           // Base HP from inspector
    public float hpIncreasePerLevel = 1f;      // How much HP increases per player level

    [HideInInspector] public float maxHealth;
    [HideInInspector] public float currentHealth;

    [Header("Death Effect")]
    public float deathScaleMultiplier = 1.4f;
    public float deathDuration = 0.4f;

    Rigidbody2D rb;
    SpriteRenderer[] renderers;
    Collider2D[] colliders;

    bool isDying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();

        // Initialize HP scaled to current player level
        int playerLevel = PointTracker.Instance != null ? PointTracker.Instance.GetPlayerLevel() : 1;
        maxHealth = baseMaxHealth + hpIncreasePerLevel * (playerLevel - 1);
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply HP increase based on current player level without resetting damage taken
    /// Call this from PointTracker.LevelUp()
    /// </summary>
    /// <param name="playerLevel"></param>
    public void ApplyLevelScalingDelta(int playerLevel)
    {
        float newMaxHealth = baseMaxHealth + hpIncreasePerLevel * (playerLevel - 1);
        float delta = newMaxHealth - maxHealth;

        if (delta <= 0f) return; // Already at or above scaled HP

        maxHealth += delta;
        currentHealth += delta;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDying) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;

        // Award points
        int totalPoints = Mathf.RoundToInt(maxHealth * 10);
        if (PointTracker.Instance != null)
            PointTracker.Instance.UpdatePointFill(totalPoints);

        // Disable collisions & physics immediately
        foreach (Collider2D col in colliders)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Splitter enemy special case
        SplitterEnemy splitter = GetComponent<SplitterEnemy>();
        if (splitter != null)
            splitter.SplitOnDeath();

        // Start visual death effect
        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        float t = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * deathScaleMultiplier;

        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;

        while (t < deathDuration)
        {
            t += Time.deltaTime;
            float p = t / deathDuration;

            transform.localScale = Vector3.Lerp(startScale, targetScale, p);

            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = renderers[i].color;
                c.a = Mathf.Lerp(startAlphas[i], 0f, p);
                renderers[i].color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
