using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage = 1)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " hit! Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        int totalPoints = maxHealth * 10;
        if (PointTracker.Instance != null)
        {
            PointTracker.Instance.UpdatePointFill(totalPoints);
        }
        Debug.Log(gameObject.name + " DIED! Total Points Earned: +" + totalPoints);
        Destroy(gameObject);
    }
}