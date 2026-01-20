using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;  // Change to 5 for tankier enemies!
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;  // Full health at spawn
    }

    public void TakeDamage(int damage = 1)  // Bullet calls this
    {
        currentHealth -= damage;
        
        // Optional: Print health (see in Console)
        Debug.Log(gameObject.name + " hit! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);  // 💥 Gone!
    }
}