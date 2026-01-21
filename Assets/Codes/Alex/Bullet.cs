using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);  // Backup: Auto-destroy after 5 secs
    }

   void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Hit"))
    {
        Health hitHealth = collision.gameObject.GetComponent<Health>();  // FIXED!
        if (hitHealth != null)
        {
            hitHealth.TakeDamage(1);
        }
        Destroy(gameObject);  // Bullet dies
    }
}
}