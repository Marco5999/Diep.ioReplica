using UnityEngine;

public class PlayerMoveAndCamera : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 20f;
    public float deceleration = 20f;

    [Header("Camera")]
    public Camera cam;
    public float cameraSmooth = 0.15f;

    Rigidbody2D rb;
    Vector2 input;
    Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // ---- MOVEMENT (physics) ----
        Vector2 targetVelocity = input.normalized * moveSpeed;

        if (input.magnitude > 0.1f)
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        else
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);

        rb.linearVelocity = currentVelocity;

        // ---- CAMERA (physics synced) ----
        Vector3 desiredCamPos = new Vector3(rb.position.x, rb.position.y, -10f);
        cam.transform.position = Vector3.Lerp(cam.transform.position, desiredCamPos, cameraSmooth);

        cam.transform.rotation = Quaternion.identity;
    }
}
