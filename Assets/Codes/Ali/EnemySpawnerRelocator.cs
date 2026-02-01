using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnerRelocator : MonoBehaviour
{
    [Header("Relocation Settings")]
    public float relocationInterval = 30f;   // editable in inspector
    public float spawnerRadius = 0.5f;
    public int placementAttempts = 50;

    [Header("Level Reference")]
    [HideInInspector] public Transform levelCenter;            // handles moved levels
    public float roomSize = 20f;

    private List<Bounds> usedBounds;

    public void Init(List<Bounds> sharedBounds, Transform center, float size)
    {
        usedBounds = sharedBounds;
        levelCenter = center;
        roomSize = size;

        StartCoroutine(RelocationLoop());
    }

    IEnumerator RelocationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(relocationInterval);
            TryRelocate();
        }
    }

    void TryRelocate()
    {
        float half = roomSize / 2f - spawnerRadius;

        for (int attempt = 0; attempt < placementAttempts; attempt++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-half, half),
                Random.Range(-half, half)
            );

            Vector2 pos = (Vector2)levelCenter.position + offset;

            Bounds newBounds = new Bounds(pos, Vector3.one * spawnerRadius * 2f);

            if (!IsOverlapping(newBounds))
            {
                transform.position = pos;
                usedBounds.Add(newBounds);
                return;
            }
        }
    }

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
