using UnityEngine;

public class CrazyTruckMovement : MonoBehaviour
{
    public Transform target;        // Drag the people object here
    public float speed = 5.0f;      // How fast the truck moves
    public float stopDistance = 2f; // How close the truck gets before stopping

    // Crazy driving parameters
    public float bounceHeight = 0.3f;     // How high the truck bounces
    public float bounceSpeed = 10f;       // How fast it bounces
    public float wobbleAmount = 5f;       // How much it wobbles side to side
    public float accelerationJitter = 2f;  // Random speed changes

    private Vector3 originalPosition;
    private float bounceOffset = 0f;
    private float wobbleOffset = 0f;
    private float speedMultiplier = 1f;

    void Start()
    {
        originalPosition = transform.position;
        // Randomize starting offsets
        bounceOffset = Random.Range(0f, 100f);
        wobbleOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Check if we have a target and we're not already too close
        if (target != null && Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            // Update our offsets
            bounceOffset += Time.deltaTime * bounceSpeed;
            wobbleOffset += Time.deltaTime * 5.0f;

            // Randomly change speed
            if (Random.Range(0f, 1f) < 0.05f) // 5% chance each frame
            {
                speedMultiplier = Random.Range(0.7f, 1.3f) * accelerationJitter;
            }

            // Calculate direction to the target (only on X and Z axes)
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep the truck at the same height
            direction.Normalize();

            // Add wobble to direction
            float wobble = Mathf.Sin(wobbleOffset) * wobbleAmount * Time.deltaTime;
            direction = Quaternion.Euler(0, wobble, 0) * direction;

            // Move the truck with jittery speed
            transform.position += direction * speed * speedMultiplier * Time.deltaTime;

            // Apply bouncing on Y axis
            float yBounce = Mathf.Sin(bounceOffset) * bounceHeight;
            transform.position = new Vector3(
                transform.position.x,
                originalPosition.y + yBounce,
                transform.position.z
            );

            // Tilt the truck based on bouncing
            float forwardTilt = Mathf.Cos(bounceOffset) * 5f;
            float sideTilt = Mathf.Sin(wobbleOffset * 2.3f) * 3f;

            // Rotate the truck to face the direction it's moving with tilt
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion tiltRotation = Quaternion.Euler(forwardTilt, 0, sideTilt);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * tiltRotation, 3.0f * Time.deltaTime);
            }
        }
    }
}