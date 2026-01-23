using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Contact Damage Setup")]
    public int damageToPlayer = 20;
    public float damageCooldown = 0.5f;

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
}