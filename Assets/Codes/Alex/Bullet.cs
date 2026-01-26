using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);  // Backup timer
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage "Hit" ONLY (enemies)
        if (collision.gameObject.CompareTag("Hit"))
        {
            Health hitHealth = collision.gameObject.GetComponent<Health>();
            if (hitHealth != null)
            {
                int damage = UpgradeManager.Instance ? UpgradeManager.Instance.GetBulletDamage() : 1;
                hitHealth.TakeDamage(damage);
            }
        }

        // DESTROY ON ANY COLLISION! (walls, player, bullets, etc.)
        Destroy(gameObject);
    }
}