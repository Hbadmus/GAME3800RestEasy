using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaminableRotate : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 1f;
    public float maxZoom = 10f;

    bool dragging = false;
    float currentZoom;
    Camera examineCamera;

    void Start()
    {
        examineCamera = GameObject.FindWithTag("ExamineCamera").GetComponent<Camera>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            dragging = true;

        if (Input.GetMouseButtonUp(0))
            dragging = false;

        if (dragging)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(Vector3.up, -mouseX, Space.World);
            transform.Rotate(Vector3.right, mouseY, Space.Self);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            Vector3 direction = (examineCamera.transform.position - transform.position).normalized;
            examineCamera.transform.position = transform.position + direction * currentZoom;
        }
    }
}
