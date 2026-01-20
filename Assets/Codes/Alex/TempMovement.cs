using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Tweak in Inspector!
    
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // WASD input
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Rotate BODY to face movement (only if moving)
        if (moveInput != Vector2.zero)
        {
            float bodyAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, bodyAngle);
        }

        // Save for physics
        movement = moveInput;
    }

    void FixedUpdate()
    {
        // Smooth glide
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}