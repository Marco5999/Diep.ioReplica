using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;       // Drag your player here
    public Vector3 offset = new Vector3(0, 20f, -0.5f); // height above level

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPos = player.position + offset;
            transform.position = newPos;

            // Keep camera looking straight down
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
