using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [HideInInspector] public EnemySpawner spawner;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Death Effect")]
    public float deathScaleMultiplier = 1.4f;
    public float deathDuration = 0.4f;

    Rigidbody2D rb;
    SpriteRenderer[] renderers;
    Collider2D[] colliders;

    bool isDying = false;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
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

        // 🔴 IMPORTANT PART — disable physics & collisions immediately
        DisablePhysics();

        StartCoroutine(DeathEffect());
    }

    void DisablePhysics()
    {
        // Disable all colliders so bullets pass through
        foreach (Collider2D col in colliders)
            col.enabled = false;

        // Stop physics movement
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
}