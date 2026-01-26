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
    public GameObject player;  // assign the existing player here
    public float playerSafeRadius = 1.5f;

    [Header("Placement Settings")]
    public int placementAttempts = 50; // max tries before giving up
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

        // Top
        CreateOuterWall(new Vector2(0, half + offset), new Vector2(roomSize + outerWallThickness * 2f, outerWallThickness));
        // Bottom
        CreateOuterWall(new Vector2(0, -half - offset), new Vector2(roomSize + outerWallThickness * 2f, outerWallThickness));
        // Left
        CreateOuterWall(new Vector2(-half - offset, 0), new Vector2(outerWallThickness, roomSize));
        // Right
        CreateOuterWall(new Vector2(half + offset, 0), new Vector2(outerWallThickness, roomSize));
    }

    void CreateOuterWall(Vector2 pos, Vector2 size)
    {
        GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);
        usedBounds.Add(new Bounds(pos, size));
    }
    #endregion

    #region Inner Walls
    void GenerateInnerWalls()
    {
        for (int i = 0; i < innerWallCount; i++)
        {
            for (int attempt = 0; attempt < placementAttempts; attempt++)
            {
                Vector2 size = new Vector2(Random.Range(1f, maxInnerWallSize.x), Random.Range(1f, maxInnerWallSize.y));
                float half = roomSize / 2f;
                Vector2 pos = new Vector2(
                    Random.Range(-half + size.x / 2f, half - size.x / 2f),
                    Random.Range(-half + size.y / 2f, half - size.y / 2f)
                );

                Bounds wallBounds = new Bounds(pos, size);

                if (!IsOverlapping(wallBounds))
                {
                    GameObject wall = Instantiate(innerWallPrefab, pos, Quaternion.identity);
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

        for (int attempt = 0; attempt < placementAttempts; attempt++)
        {
            float half = roomSize / 2f;
            Vector2 pos = new Vector2(
                Random.Range(-half, half),
                Random.Range(-half, half)
            );

            Bounds playerBounds = new Bounds(pos, Vector3.one * playerSafeRadius * 2f);

            if (!IsOverlapping(playerBounds))
            {
                player.transform.position = pos;
                usedBounds.Add(playerBounds);
                break;
            }
        }
    }
    #endregion

    #region Enemy Spawners
    void GenerateEnemySpawners()
    {
        for (int i = 0; i < spawnerCount; i++)
        {
            for (int attempt = 0; attempt < placementAttempts; attempt++)
            {
                float half = roomSize / 2f;
                Vector2 pos = new Vector2(
                    Random.Range(-half, half),
                    Random.Range(-half, half)
                );

                Bounds spawnerBounds = new Bounds(pos, Vector3.one * spawnerRadius * 2f);

                if (!IsOverlapping(spawnerBounds))
                {
                    Instantiate(enemySpawnerPrefab, pos, Quaternion.identity);
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
