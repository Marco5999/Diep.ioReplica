using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyType
{
    public GameObject prefab;       // Enemy prefab
    public float spawnWeight = 1f;  // Higher = more likely to spawn
    public int maxPerSpawner = 99;  // Max of this type alive at once
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<EnemyType> enemyTypes = new List<EnemyType>();
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
            // Clean up null entries (destroyed enemies)
            aliveEnemies.RemoveAll(e => e == null);

            // Spawn if under max alive
            if (aliveEnemies.Count < maxAliveEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyTypes.Count == 0) return;

        GameObject chosenPrefab = GetWeightedRandomEnemy();
        if (chosenPrefab == null) return;

        GameObject enemy = Instantiate(chosenPrefab, transform.position, Quaternion.identity);

        // Assign spawner reference for Health
        Health health = enemy.GetComponent<Health>();
        if (health != null)
        {
            health.spawner = this;
        }

        aliveEnemies.Add(enemy);
    }

    GameObject GetWeightedRandomEnemy()
    {
        float totalWeight = 0f;

        // Compute total weight considering maxPerSpawner
        foreach (var type in enemyTypes)
        {
            int currentCount = aliveEnemies.FindAll(e => e != null && e.name.Contains(type.prefab.name)).Count;
            if (currentCount < type.maxPerSpawner)
                totalWeight += type.spawnWeight;
        }

        if (totalWeight <= 0f) return null;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var type in enemyTypes)
        {
            int currentCount = aliveEnemies.FindAll(e => e != null && e.name.Contains(type.prefab.name)).Count;
            if (currentCount >= type.maxPerSpawner) continue;

            cumulative += type.spawnWeight;
            if (randomValue <= cumulative)
                return type.prefab;
        }

        return null;
    }

    // Called from Health when enemy dies
    public void OnEnemyDied(GameObject enemy)
    {
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);
    }
}
