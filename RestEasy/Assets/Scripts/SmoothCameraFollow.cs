using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;         // The character's transform
    public float smoothSpeed = 10f;  // Higher = smoother camera

    private Vector3 offset;

    void Start()
    {
        // Store initial offset between camera and target
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Only use X and Z movement from character, keep Y stable
        desiredPosition.y = transform.position.y;
        
        // Smoothly move to that position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}