using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Vision")]
    public float visionRange = 8f;
    public LayerMask wallLayer;

    [Header("Movement")]
    public float moveSpeed = 3f;

    Rigidbody2D rb;
    Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find player at runtime (prefab-safe)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        // 1. Too far → stop
        if (distance > visionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 2. Raycast to check wall blocking view
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            toPlayer.normalized,
            distance,
            wallLayer
        );

        // Debug vision line
        Debug.DrawRay(transform.position, toPlayer, Color.red);

        // If wall hit → stop
        if (hit.collider != null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 3. Chase player
        rb.linearVelocity = toPlayer.normalized * moveSpeed;
    }
}
