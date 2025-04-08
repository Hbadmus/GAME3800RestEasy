using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float gravity = 9.81f;
    public float mouseSensitivity = 2f;

    public Transform cameraTransform; // First-person camera reference
    private CharacterController controller;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalRotation = 0f;

    private bool isPossessed = false;
    private float normalSpeed;
    private float normalSensitivity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //Cursor.lockState = CursorLockMode.Locked;

        //normalSpeed = speed;
        //normalSensitivity = mouseSensitivity;
    }

    void Update()
    {
        if (!isPossessed)
        {
            HandleMovement();
            HandleMouseLook();
        }

    }

    public void BecomePossessed()
    {
        isPossessed = true;

        // Disable movement control
        speed = 0f;
        mouseSensitivity = 0f;

    }

    public void EndPossession()
    {
        isPossessed = false;

        // Restore movement control
        speed = normalSpeed;
        mouseSensitivity = normalSensitivity;
    }

        void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * moveVertical;
        Vector3 rightMovement = transform.right * moveHorizontal;

        Vector3 input = (forwardMovement + rightMovement).normalized * speed;

        if (controller.isGrounded)
        {
            moveDirection = input;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the character
        controller.Move(moveDirection * Time.deltaTime);

        // Animation states
        animator.SetBool("isWalking", moveVertical > 0);
        animator.SetBool("isWalkingBackward", moveVertical < 0);
        animator.SetBool("isStrafingRight", moveHorizontal > 0);
        animator.SetBool("isStrafingLeft", moveHorizontal < 0);
    }

    void HandleMouseLook()
    {        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically, clamping to prevent flipping
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
