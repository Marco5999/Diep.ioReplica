using UnityEngine;

public class PlayerHealth1 : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;
    public float currentHP;
    public float contactDamage = 10f;
    public float damageCooldown = 0.5f;

    [Header("HP Bar Parts")]
    public Transform fillMiddle;    // Middle rectangle
    public Transform fillRightCap;  // Right circle
    public Transform fillLeftCap;   // Left circle

    [Header("HP Bar Settings")]
    public float smoothSpeed = 12f; // How fast the bar shrinks

    private float nextDamageTime;
    private float middleOriginalWidth;

    void Start()
    {
        currentHP = maxHP;

        if (fillMiddle != null)
            middleOriginalWidth = fillMiddle.localScale.x;
    }

    void Update()
    {
        if (fillMiddle == null) return;

        // Calculate HP percentage
        float hpPercent = currentHP / maxHP;

        // Smoothly shrink the middle fill
        float targetWidth = middleOriginalWidth * hpPercent;
        Vector3 midScale = fillMiddle.localScale;
        midScale.x = Mathf.Lerp(midScale.x, targetWidth, Time.deltaTime * smoothSpeed);
        fillMiddle.localScale = midScale;

        // Keep left edge fixed (pivot is left)
        fillMiddle.localPosition = new Vector3(-middleOriginalWidth / 2f, fillMiddle.localPosition.y, 0);

        // Left circle fixed at left edge
        if (fillLeftCap != null)
            fillLeftCap.localPosition = new Vector3(-middleOriginalWidth / 2f, fillLeftCap.localPosition.y, 0);

        // Right circle sticks to right edge of middle fill
        if (fillRightCap != null)
        {
            float currentFillWidth = midScale.x;
            fillRightCap.localPosition = new Vector3(-middleOriginalWidth / 2f + currentFillWidth, fillRightCap.localPosition.y, 0);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hit") && Time.time >= nextDamageTime)
        {
            TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHP = Mathf.Clamp(currentHP - dmg, 0f, maxHP);
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0f, maxHP);
    }
}
