using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaminableRotate : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float zoomSpeed = 2f;
    public float maxCameraPos = -10f;
    public float minCameraPos = -40f;

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
            currentZoom -= scroll * zoomSpeed / 10f;

            // Vector3 direction = (examineCamera.transform.position - transform.position).normalized;
            if (scroll > 0)
            {
                Vector3 newCameraPos = examineCamera.transform.position - Vector3.forward * currentZoom;
                newCameraPos.z = Mathf.Clamp(newCameraPos.z, minCameraPos, maxCameraPos);
                examineCamera.transform.position = newCameraPos;
            }
            else
            {
                Vector3 newCameraPos = examineCamera.transform.position + Vector3.forward * currentZoom;
                newCameraPos.z = Mathf.Clamp(newCameraPos.z, minCameraPos, maxCameraPos);
                examineCamera.transform.position = newCameraPos;
            }
        }
    }
}
