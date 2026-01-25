using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public float roomSize = 20f;

    [Header("Outer Walls")]
    public GameObject outerWallPrefab;
    public float outerWallThickness = 1f;

    [Header("Inner Walls")]
    public GameObject wallPrefab;
    public int wallCount = 10;
    public float maxWallSize = 6f;

    [Header("Player (EXISTING IN SCENE)")]
    public Transform player;

    [Header("Placement Settings")]
    public int placementAttempts = 50;

    private List<Bounds> usedBounds = new List<Bounds>();

    void Start()
    {
        GenerateOuterWalls();
        GenerateInnerWalls();
        PlaceExistingPlayer();
    }

    // ================= OUTER WALLS =================

    void GenerateOuterWalls()
    {
        float half = roomSize / 2f;
        float offset = outerWallThickness / 2f;

        // Top
        CreateOuterWall(
            new Vector2(0, half + offset),
            new Vector2(roomSize + outerWallThickness * 2f, outerWallThickness)
        );

        // Bottom
        CreateOuterWall(
            new Vector2(0, -half - offset),
            new Vector2(roomSize + outerWallThickness * 2f, outerWallThickness)
        );

        // Left
        CreateOuterWall(
            new Vector2(-half - offset, 0),
            new Vector2(outerWallThickness, roomSize)
        );

        // Right
        CreateOuterWall(
            new Vector2(half + offset, 0),
            new Vector2(outerWallThickness, roomSize)
        );
    }

    void CreateOuterWall(Vector2 position, Vector2 size)
    {
        GameObject wall = Instantiate(outerWallPrefab, position, Quaternion.identity);
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        Bounds bounds = new Bounds(position, size);
        usedBounds.Add(bounds);
    }

    // ================= INNER WALLS =================

    void GenerateInnerWalls()
    {
        for (int i = 0; i < wallCount; i++)
        {
            for (int attempt = 0; attempt < placementAttempts; attempt++)
            {
                float sizeX = Random.Range(1f, maxWallSize);
                float sizeY = Random.Range(1f, maxWallSize);
                Vector2 size = new Vector2(sizeX, sizeY);

                float half = roomSize / 2f - Mathf.Max(sizeX, sizeY) / 2f;
                Vector2 pos = new Vector2(
                    Random.Range(-half, half),
                    Random.Range(-half, half)
                );

                Bounds newBounds = new Bounds(pos, size);

                if (!IsOverlapping(newBounds))
                {
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(sizeX, sizeY, 1f);

                    usedBounds.Add(newBounds);
                    break;
                }
            }
        }
    }

    // ================= PLAYER =================

    void PlaceExistingPlayer()
    {
        if (player == null)
        {
            Debug.LogError("LevelGenerator: Player reference is missing!");
            return;
        }

        for (int attempt = 0; attempt < placementAttempts; attempt++)
        {
            float half = roomSize / 2f;
            Vector2 pos = new Vector2(
                Random.Range(-half, half),
                Random.Range(-half, half)
            );

            Bounds playerBounds = new Bounds(pos, Vector3.one);

            if (!IsOverlapping(playerBounds))
            {
                player.position = pos;
                return;
            }
        }

        Debug.LogWarning("Player could not find a safe spawn position.");
    }

    // ================= UTILS =================

    bool IsOverlapping(Bounds bounds)
    {
        foreach (Bounds b in usedBounds)
        {
            if (b.Intersects(bounds))
                return true;
        }
        return false;
    }
}
