using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public bool isClicked;

    [Header("Slide Settings")]
    [SerializeField] private float pushInDistance = 0.05f;
    [SerializeField] private float slideDuration = 0.2f;

    private Vector3 originalPosition;
    private bool isAnimating = false;

    void Start()
    {
        isClicked = false;
        originalPosition = transform.position;
    }

    public void OnMouseDown()
    {
        if (!isAnimating)
        {
            StartCoroutine(PushInAnimation());
        }
    }

    private IEnumerator PushInAnimation()
    {
        isAnimating = true;

        // Push directly along positive Z axis
        Vector3 pushDirection = Vector3.forward; // This is (0, 0, 1)

        // Push in
        Vector3 targetPosition = originalPosition + pushDirection * pushInDistance;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach the exact target position
        transform.position = targetPosition;

        isAnimating = false;
    }
}