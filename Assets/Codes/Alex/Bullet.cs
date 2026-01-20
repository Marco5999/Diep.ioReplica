using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);  // Backup: Auto-destroy after 5 secs
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit something tagged "Hit"? Destroy IT + bullet!
      if (collision.gameObject.CompareTag("Hit"))
    {
        // Find Health script + hurt it
        Health hitHealth = collision.gameObject.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.TakeDamage(1);  // Pew! -1 health
        }
        
        Destroy(gameObject);  // Bullet always dies on hit
    }
    }
}