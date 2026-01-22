using UnityEngine;

public class PlayerMoveAndCamera : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 20f;
    public float deceleration = 20f;

    [Header("Recoil Push (Diep.io Slippery)")]
    public float recoilDeceleration = 12f;  // SLOWER than decel = slippery linger!

    [Header("Camera")]
    public Camera cam;
    public float cameraSmooth = 0.15f;

    Rigidbody2D rb;
    Vector2 input;
    Vector2 currentVelocity;
    Vector2 recoilAccum;  // Accumulates pushes, damps smoothly

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // TOP-DOWN LOCK (0 grav, no spin)
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // RECOIL-BLENDED TARGET (smooth slippery push!)
        Vector2 targetVelocity = input.normalized * moveSpeed + recoilAccum;

        // Accel/decel to blended target (your original smooth!)
        if (input.magnitude > 0.1f)
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        else
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, deceleration * Time.fixedDeltaTime);  // Includes recoil!

        rb.linearVelocity = currentVelocity;

        // DAMP RECOIL SLIPPERY (lingers like diep.io)
        recoilAccum = Vector2.MoveTowards(recoilAccum, Vector2.zero, recoilDeceleration * Time.fixedDeltaTime);

        // CAMERA SMOOTH FOLLOW (your delay ref – buttery!)
        Vector3 desiredCamPos = new Vector3(rb.position.x, rb.position.y, -10f);
        cam.transform.position = Vector3.Lerp(cam.transform.position, desiredCamPos, cameraSmooth);
        cam.transform.rotation = Quaternion.identity;
    }

    // PUBLIC: Called by Shooting on each shot
    public void AddRecoil(Vector2 recoilDir)
    {
        recoilAccum += recoilDir;
    }
}