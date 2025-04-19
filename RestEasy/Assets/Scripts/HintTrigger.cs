using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [Header("Hint Settings")]
    public string hintID = ""; // The ID of the hint to reveal
    public bool triggerOnce = true;    // Should this only trigger once?

    [Header("Trigger Method")]
    public bool triggerOnInteract = true;  // Trigger when player interacts
    public bool triggerOnProximity = false; // Trigger when player gets close
    public float proximityDistance = 2f;    // How close player needs to be

    private bool hasTriggered = false;
    private NotebookController notebook;
    private Transform playerTransform;

    private void Start()
    {
        notebook = FindObjectOfType<NotebookController>();
        if (notebook == null)
        {
            Debug.LogError("NotebookController not found in scene!");
        }

        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else if (triggerOnProximity)
        {
            Debug.LogWarning("Player not found! Proximity triggering won't work.");
        }
    }

    private void Update()
    {
        // Check for proximity trigger
        if (triggerOnProximity && !hasTriggered && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= proximityDistance)
            {
                RevealHint();
            }
        }
    }

    // Call this method when the player interacts with the object
    public void OnInteract()
    {
        if (triggerOnInteract && !hasTriggered)
        {
            RevealHint();
        }
    }

    private void RevealHint()
    {
        if (notebook != null)
        {
            notebook.EnableHint(hintID);

            if (triggerOnce)
            {
                hasTriggered = true;
            }
        }
    }

    // Visual representation of trigger area in editor
    private void OnDrawGizmosSelected()
    {
        if (triggerOnProximity)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, proximityDistance);
        }
    }
}