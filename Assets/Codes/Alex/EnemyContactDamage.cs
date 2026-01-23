using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Contact Damage Setup")]
    public int damageToPlayer = 20;      // How much HP player loses per touch (tune 10-30)
    public float damageCooldown = 0.5f;  // Prevent spam damage (like diep.io tick rate)

    private float lastDamageTime;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Cooldown check (prevents 60+ dmg/sec if stuck)
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
}