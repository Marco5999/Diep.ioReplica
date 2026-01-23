using UnityEngine;

public class PlayerRammingDamage : MonoBehaviour
{
    [Header("Ram Damage Setup")]
    public int ramDamage = 1;            // HP loss to enemy per ram (1 = like bullet)
    public float ramCooldown = 0.5f;     // Prevent spam damage

    private float lastRamTime;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ram "Hit" tagged enemies!
        if (collision.gameObject.CompareTag("Hit"))
        {
            if (Time.time - lastRamTime < ramCooldown) return;

            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(ramDamage);
                lastRamTime = Time.time;

                Debug.Log("Player RAM! Enemy took " + ramDamage + " damage.");
            }
        }
    }
}