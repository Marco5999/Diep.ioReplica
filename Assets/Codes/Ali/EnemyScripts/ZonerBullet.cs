using UnityEngine;

public class ZonerBullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // cleanup
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit PLAYER only
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Destroy on ANY collision (walls, player, etc.)
        Destroy(gameObject);
    }
}
