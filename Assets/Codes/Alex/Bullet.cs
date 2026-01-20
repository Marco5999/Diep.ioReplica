using UnityEngine;

public class Bullet : MonoBehaviour
{
   void Start()
    {
        Destroy(gameObject, 5f);  // Destroy after 5 seconds
    }
void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the thing we hit has the "Hit" tag
        if (collision.gameObject.CompareTag("Hit"))
        {
            Destroy(gameObject);  // Bullet goes POOF!
            // Optional: Destroy the hit thing too? Uncomment next line:
            // Destroy(collision.gameObject);
        }
    }
    // Optional: If it hits something, destroy it
  
}
