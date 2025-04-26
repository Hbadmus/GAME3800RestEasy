using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 6f;
    [SerializeField] private Camera playerCamera;

    void Start()
    {
        // If no camera is manually assigned, try to get camera from children
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        // Hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }

        // Optional: Allow cursor to reappear if needed (e.g., when pausing)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HandleInteraction()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("" + hitObject.name);

            // Check for emotional significance
            EmotionalObject emotionalObj = hitObject.GetComponent<EmotionalObject>();
            if (emotionalObj != null && !string.IsNullOrEmpty(emotionalObj.emotionalTag))
            {
                // Notify the ghost AI about this interaction
                GhostAI.Instance.OnPlayerInteractWithEmotionalObject(emotionalObj.emotionalTag);
            }

            // Handle specific object interactions based on tags
            switch (hitObject.tag)
            {

                case "GumballMachine":
                    // Interact with gumball machine
                    AudioManager.instance.PlaySFX("coin-dispensing");
                    GumballPuzzle gumballPuzzle = hitObject.GetComponent<GumballPuzzle>();
                    if (gumballPuzzle != null)
                    {
                        gumballPuzzle.CompleteGumballPuzzle();
                    }
                    break;

                case "Book":
                    // Handle book interaction
                    Debug.Log("Hit book");
                    BookshelfPuzzle bookshelfPuzzle = GameObject.Find("BookshelfManager")?.GetComponent<BookshelfPuzzle>();
                    if (bookshelfPuzzle != null)
                    {
                        bookshelfPuzzle.IncrementBooksCollected(hitObject);
                    }
                    break;

                default:
                    // Optional: Add any default interaction logic
                    break;
            }
        }
    }
}