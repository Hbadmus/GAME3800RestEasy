using UnityEngine;

public class ImportantDoor : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;
    [SerializeField] private Animator doorAnimator;

    private void Start()
    {
        // Find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Get animator component if not assigned
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }
    }

    private void OnMouseDown()
    {
        // Check if all keys are collected
        if (gameManager.AreAllKeysCollected())
        {

            // Trigger door animation
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("openDoor");


            }
            else
            {
                Debug.LogError("Door animator not found on " + gameObject.name);
            }
        }
    }
    
}