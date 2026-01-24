using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;
    public int maxAliveEnemies = 8;

    int currentAliveEnemies = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentAliveEnemies < maxAliveEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Assign spawner reference
        Health health = enemy.GetComponent<Health>();
        if (health != null)
        {
            health.spawner = this;
        }

        currentAliveEnemies++;
    }

    public void OnEnemyDied()
    {
        currentAliveEnemies--;
        currentAliveEnemies = Mathf.Max(0, currentAliveEnemies);
    }
}
