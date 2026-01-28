using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [HideInInspector] public EnemySpawner spawner;

    [Header("Health")]
    public int maxHealth = 3;              // Inspector starting value
    private int currentHealth;

    [Header("Death Effect")]
    public float deathScaleMultiplier = 1.4f;
    public float deathDuration = 0.4f;

    [Header("HP Scaling per Level")]
    public float hpIncreasePerLevel = 1f;  // Set per enemy type (Red=1, Splitter=1.5, Zoner=0.5)

    Rigidbody2D rb;
    SpriteRenderer[] renderers;
    Collider2D[] colliders;

    bool isDying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();

        // Apply scaling for current player level
        if (PointTracker.Instance != null)
        {
            int playerLevel = PointTracker.Instance.GetPlayerLevel();
            if (playerLevel > 1)
                ScaleHP(playerLevel - 1);
        }

        // Initialize current health
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDying) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;

        // Award points
        int totalPoints = maxHealth * 10;
        if (PointTracker.Instance != null)
        {
            PointTracker.Instance.UpdatePointFill(totalPoints);
        }

        Debug.Log(gameObject.name + " DIED! +" + totalPoints + " points");

        // Disable collisions & physics immediately
        DisablePhysics();

        // Splitter special
        SplitterEnemy splitter = GetComponent<SplitterEnemy>();
        if (splitter != null)
            splitter.SplitOnDeath();

        // Start death visual effect
        StartCoroutine(DeathEffect());
    }

    void DisablePhysics()
    {
        foreach (Collider2D col in colliders)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
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

    // Call this whenever player levels up to scale all existing enemies
    public void ScaleHP(int levelsGained)
    {
        // Increase max health
        float addedHP = hpIncreasePerLevel * levelsGained;
        maxHealth += Mathf.RoundToInt(addedHP);

        // Increase current health proportionally (without resetting damage)
        currentHealth += Mathf.RoundToInt(addedHP);
    }

    // Static helper to scale all existing enemies when player levels up
    public static void ScaleExistingEnemiesHP(int levelsGained)
    {
        Health[] allEnemies = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);
        foreach (var e in allEnemies)
        {
            e.ScaleHP(levelsGained);
        }
    }
}
