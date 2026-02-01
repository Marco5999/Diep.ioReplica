using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public float roomSize = 20f;

    [Header("Outer Walls")]
    public GameObject wallPrefab;
    public float outerWallThickness = 1f;

    [Header("Inner Walls")]
    public GameObject innerWallPrefab;
    public int innerWallCount = 10;
    public Vector2 maxInnerWallSize = new Vector2(6f, 6f);

    [Header("Enemy Spawner Settings")]
    public GameObject enemySpawnerPrefab;
    public int spawnerCount = 1;
    public float spawnerRadius = 0.5f;

    [Header("Player Settings")]
    public GameObject player;
    public float playerSafeRadius = 1.5f;

    [Header("Placement Settings")]
    public int placementAttempts = 50;

    // shared collision data
    private List<Bounds> usedBounds = new List<Bounds>();

    void Start()
    {
        GenerateOuterWalls();
        GenerateInnerWalls();
        PlaceExistingPlayer();
        GenerateEnemySpawners();
    }

    #region Outer Walls
    void GenerateOuterWalls()
    {
        float half = roomSize / 2f;
        float offset = outerWallThickness / 2f;

        CreateOuterWall(new Vector2(0, half + offset), new Vector2(roomSize + outerWallThickness * 2f, outerWallThickness));
        CreateOuterWall(new Vector2(0, -half - offset), new Vector2(roomSize + outerWallThickness * 2f, outerWallThickness));
        CreateOuterWall(new Vector2(-half - offset, 0), new Vector2(outerWallThickness, roomSize));
        CreateOuterWall(new Vector2(half + offset, 0), new Vector2(outerWallThickness, roomSize));
    }

    void CreateOuterWall(Vector2 localPos, Vector2 size)
    {
        Vector2 worldPos = (Vector2)transform.position + localPos;

        GameObject wall = Instantiate(wallPrefab, worldPos, Quaternion.identity);
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        usedBounds.Add(new Bounds(worldPos, size));
    }
    #endregion

    #region Inner Walls
    void GenerateInnerWalls()
    {
        float half = roomSize / 2f;

        for (int i = 0; i < innerWallCount; i++)
        {
            for (int attempt = 0; attempt < placementAttempts; attempt++)
            {
                Vector2 size = new Vector2(
                    Random.Range(1f, maxInnerWallSize.x),
                    Random.Range(1f, maxInnerWallSize.y)
                );

                Vector2 localPos = new Vector2(
                    Random.Range(-half + size.x / 2f, half - size.x / 2f),
                    Random.Range(-half + size.y / 2f, half - size.y / 2f)
                );

                Vector2 worldPos = (Vector2)transform.position + localPos;
                Bounds wallBounds = new Bounds(worldPos, size);

                if (!IsOverlapping(wallBounds))
                {
                    GameObject wall = Instantiate(innerWallPrefab, worldPos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(size.x, size.y, 1f);

                    usedBounds.Add(wallBounds);
                    break;
                }
            }
        }
    }
    #endregion

    #region Player Placement
    void PlaceExistingPlayer()
    {
        if (player == null) return;

        float half = roomSize / 2f;

        for (int attempt = 0; attempt < placementAttempts; attempt++)
        {
            Vector2 localPos = new Vector2(
                Random.Range(-half, half),
                Random.Range(-half, half)
            );

            Vector2 worldPos = (Vector2)transform.position + localPos;
            Bounds playerBounds = new Bounds(worldPos, Vector3.one * playerSafeRadius * 2f);

            if (!IsOverlapping(playerBounds))
            {
                player.transform.position = worldPos;
                usedBounds.Add(playerBounds);
                break;
            }
        }
    }
    #endregion

    #region Enemy Spawners
    void GenerateEnemySpawners()
    {
        float half = roomSize / 2f;

        for (int i = 0; i < spawnerCount; i++)
        {
            for (int attempt = 0; attempt < placementAttempts; attempt++)
            {
                Vector2 localPos = new Vector2(
                    Random.Range(-half, half),
                    Random.Range(-half, half)
                );

                Vector2 worldPos = (Vector2)transform.position + localPos;
                Bounds spawnerBounds = new Bounds(worldPos, Vector3.one * spawnerRadius * 2f);

                if (!IsOverlapping(spawnerBounds))
                {
                    GameObject spawner = Instantiate(enemySpawnerPrefab, worldPos, Quaternion.identity);

                    EnemySpawnerRelocator relocator = spawner.GetComponent<EnemySpawnerRelocator>();
                    if (relocator != null)
                    {
                        relocator.spawnerRadius = spawnerRadius;
                        relocator.placementAttempts = placementAttempts;
                        relocator.Init(usedBounds, transform, roomSize);
                    }

                    usedBounds.Add(spawnerBounds);
                    break;
                }
            }
        }
    }
    #endregion

    #region Utility
    bool IsOverlapping(Bounds bounds)
    {
        foreach (Bounds b in usedBounds)
        {
            if (b.Intersects(bounds))
                return true;
        }
        return false;
    }
    #endregion
}
