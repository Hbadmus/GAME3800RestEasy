using UnityEngine;
using System.Collections.Generic;

public class CrazyTruckMovement : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();  // List of waypoints to visit
    public float speed = 5.0f;                                // How fast the truck moves
    public float stopDistance = 2f;                           // How close before moving to next waypoint
    public float finalStopDistance = 3f;                      // Distance to stop from final target
    public Transform player;                                  // Reference to player transform

    // Crazy driving parameters
    public float bounceHeight = 0.3f;     // How high the truck bounces
    public float bounceSpeed = 10f;       // How fast it bounces
    public float wobbleAmount = 5f;       // How much it wobbles side to side
    public float accelerationJitter = 2f;  // Random speed changes

    private Vector3 originalPosition;
    private float bounceOffset = 0f;
    private float wobbleOffset = 0f;
    private float speedMultiplier = 1f;
    private int currentWaypointIndex = 0;  // Track which waypoint we're heading to
    private bool reachedFinalWaypoint = false;  // Flag to track if we've reached the end

    void Start()
    {
        originalPosition = transform.position;
        // Randomize starting offsets
        bounceOffset = Random.Range(0f, 100f);
        wobbleOffset = Random.Range(0f, 100f);

        // Make sure we have at least one waypoint
        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints assigned to CrazyTruckMovement!");
        }

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        // Check if we have any waypoints or if we've reached the final one
        if (waypoints.Count == 0 || reachedFinalWaypoint) return;

        // If we're at the last waypoint and close to player, stop
        if (currentWaypointIndex == waypoints.Count - 1 && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= finalStopDistance)
            {
                reachedFinalWaypoint = true;
                Debug.Log("Stopped truck 3 units away from player");

                // Reset to stable position
                transform.position = new Vector3(
                    transform.position.x,
                    originalPosition.y, // Reset to original height
                    transform.position.z
                );

                // Keep facing the player
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }

                return;
            }
        }

        // Get current target waypoint
        Transform currentTarget = waypoints[currentWaypointIndex];

        // Check if we're not already too close to the current waypoint
        if (Vector3.Distance(transform.position, currentTarget.position) > stopDistance)
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
            Vector3 direction = currentTarget.position - transform.position;
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
        else
        {
            // We've reached the current waypoint
            if (currentWaypointIndex < waypoints.Count - 1)
            {
                // Move to the next waypoint if we're not at the last one
                currentWaypointIndex++;
                Debug.Log("Reached waypoint " + currentWaypointIndex + ", moving to next waypoint");
            }
        }
    }
}