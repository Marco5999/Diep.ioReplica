using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Contact Damage Setup")]
    public int damageToPlayer = 20;
    public float damageCooldown = 0.5f;

    [Header("Damage Scaling per Level")]
    public int damageIncreasePerLevel = 5;  // Set per enemy type

    private float lastDamageTime;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime < damageCooldown) return;

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                lastDamageTime = Time.time;
                Debug.Log("Enemy contact! Player took " + damageToPlayer + " damage.");
            }
        }
    }

    // Called for a single enemy when player levels up
    public void ScaleDamage(int levelsGained)
    {
        damageToPlayer += damageIncreasePerLevel * levelsGained;
    }

    // --- STATIC HELPER ---
    public static void ScaleExistingEnemiesDamage(int levelsGained)
    {
        EnemyContactDamage[] allEnemies = Object.FindObjectsByType<EnemyContactDamage>(FindObjectsSortMode.None);
        foreach (var e in allEnemies)
        {
            e.ScaleDamage(levelsGained);
        }
    }
}
