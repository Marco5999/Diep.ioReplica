using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;
    public int maxAliveEnemies = 8;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Clean up null entries (enemies that were destroyed without notifying)
            aliveEnemies.RemoveAll(e => e == null);

            // Spawn if under max
            if (aliveEnemies.Count < maxAliveEnemies)
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

        aliveEnemies.Add(enemy);
    }

    // Optional: keep for Health calls if you want
    public void OnEnemyDied(GameObject enemy)
    {
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);
    }
}