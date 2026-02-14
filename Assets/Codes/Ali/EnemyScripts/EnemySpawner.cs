using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyType
{
    public GameObject prefab;
    public float spawnWeight = 1f;

    [Header("Base Limits")]
    public int baseMaxPerSpawner = 3;

    [Header("Scaling")]
    public int increaseMaxAlivePerThisType = 1;

    [HideInInspector] public int runtimeMaxPerSpawner;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<EnemyType> enemyTypes = new List<EnemyType>();
    public float spawnInterval = 1f;

    [Header("Base Enemy Limits")]
    public int baseMaxAliveEnemies = 8;

    [Header("Difficulty Scaling")]
    [Tooltip("Player levels needed for ONE scaling step")]
    public int levelsPerIncrease = 3;

    [Tooltip("How much max alive enemies increases per step")]
    public int increaseMaxAliveEnemies = 1;

    private int runtimeMaxAliveEnemies;
    private int lastAppliedStep = -1;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    void Start()
    {
        ApplyScaling();
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        ApplyScaling();
    }

    void ApplyScaling()
    {
        if (PointTracker.Instance == null) return;

        int playerLevel = PointTracker.Instance.GetPlayerLevel();
        int currentStep = playerLevel / levelsPerIncrease;

        if (currentStep == lastAppliedStep) return;
        lastAppliedStep = currentStep;

        // Scale total enemies
        runtimeMaxAliveEnemies = baseMaxAliveEnemies + (currentStep * increaseMaxAliveEnemies);

        // Scale each enemy type
        foreach (var type in enemyTypes)
        {
            type.runtimeMaxPerSpawner = type.baseMaxPerSpawner + (currentStep * type.increaseMaxAlivePerThisType);
        }

        // Apply scaling to existing enemies immediately
        Health.ScaleExistingEnemiesHP(currentStep * 1); // Multiply by 1 level per step
        EnemyContactDamage.ScaleExistingEnemiesDamage(currentStep * 1);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            aliveEnemies.RemoveAll(e => e == null);

            if (aliveEnemies.Count < runtimeMaxAliveEnemies)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        GameObject prefab = GetWeightedRandomEnemy();
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, transform.position, Quaternion.identity);

        // Apply Health scaling
        Health health = enemy.GetComponent<Health>();
        if (health != null)
        {
            health.spawner = this;

            if (PointTracker.Instance != null)
            {
                int playerLevel = PointTracker.Instance.GetPlayerLevel();
                if (playerLevel > 1)
                    health.ScaleHP(playerLevel - 1); // Apply all scaling up to current level
            }
        }

        // Apply Damage scaling
        EnemyContactDamage damageComp = enemy.GetComponent<EnemyContactDamage>();
        if (damageComp != null && PointTracker.Instance != null)
        {
            int playerLevel = PointTracker.Instance.GetPlayerLevel();
            if (playerLevel > 1)
                damageComp.ScaleDamage(playerLevel - 1); // Apply all scaling up to current level
        }

        aliveEnemies.Add(enemy);
    }

    GameObject GetWeightedRandomEnemy()
    {
        float totalWeight = 0f;

        foreach (var type in enemyTypes)
        {
            int count = aliveEnemies.FindAll(
                e => e != null && e.name.Contains(type.prefab.name)
            ).Count;

            if (count < type.runtimeMaxPerSpawner)
                totalWeight += type.spawnWeight;
        }

        if (totalWeight <= 0f) return null;

        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var type in enemyTypes)
        {
            int count = aliveEnemies.FindAll(
                e => e != null && e.name.Contains(type.prefab.name)
            ).Count;

            if (count >= type.runtimeMaxPerSpawner) continue;

            cumulative += type.spawnWeight;
            if (rand <= cumulative)
                return type.prefab;
        }

        return null;
    }

    public void OnEnemyDied(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }
}
