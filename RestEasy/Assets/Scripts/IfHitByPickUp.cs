using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfHitByPickUp : MonoBehaviour
{
    [Tooltip("Tag of the pickup truck that can break the fence")]
    public string pickupTruckTag = "PickupTruck";

    private Rigidbody rb;
    private Collider fenceCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        fenceCollider = GetComponent<Collider>();

        // Make sure the fence starts as kinematic until hit
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the pickup truck
        if (collision.gameObject.CompareTag(pickupTruckTag))
        {
            // Make the fence non-kinematic so physics takes over
            rb.isKinematic = false;

            // Add force based on collision impulse
            rb.AddForce(collision.impulse * 2, ForceMode.Impulse);

            // Disable the collider so nothing else can hit this fence part
            if (fenceCollider != null)
            {
                fenceCollider.enabled = false;
                fenceCollider.enabled = false;
            }

            // Disable this script after being hit once
            this.enabled = false;
        }
    }
}