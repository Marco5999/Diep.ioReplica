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
    bool isDying = false;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
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

        // Stop physics completely
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Disable all colliders
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
            col.enabled = false;

        if (spawner != null)
        {
            spawner.OnEnemyDied();
        }

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

            // Scale up smoothly
            transform.localScale = Vector3.Lerp(startScale, targetScale, p);

            // Fade all sprites (including children)
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