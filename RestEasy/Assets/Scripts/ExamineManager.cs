using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineManager : MonoBehaviour
{
    public Transform examinablePivot;
    public float displayPadding = 1.2f;
    public static ExamineManager Instance;

    Camera mainCamera;
    Camera examineCamera;
    GameObject examinableObject;

    void Awake() {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        examineCamera = GameObject.FindWithTag("ExamineCamera").GetComponent<Camera>();

        if (!examineCamera)
            Debug.LogWarning("ExamineManager could not find the examine camera -- make sure examine camera is tagged 'ExamineCamera'");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Instance.ExitExamine();
    }

    public void ExamineObject(GameObject obj)
    {
        examinableObject = Instantiate(obj, transform.position, transform.rotation, examinablePivot);
        Destroy(examinableObject.GetComponent<ExaminableObject>());

        examinableObject.layer = 3;
        // FrameExaminableObject();

        examineCamera.enabled = true;
        mainCamera.enabled = false;
        Time.timeScale = 0;
    }

    void ExitExamine()
    {
        Destroy(examinableObject);
        examinableObject = null;
        examinablePivot.transform.localRotation = Quaternion.identity;

        mainCamera.enabled = true;
        examineCamera.enabled = false;
        Time.timeScale = 1;
    }

    // this does not work 
    void FrameExaminableObject()
    {
        if (!examinableObject)
            return;

        examinableObject.transform.localPosition = Vector3.zero;
        examinableObject.transform.localRotation = Quaternion.identity;

        Bounds bounds = GetObjectBounds();

        Vector3 boundsCenter = bounds.center;
        float boundsRadius = bounds.extents.magnitude;

        float fov = examineCamera.fieldOfView * Mathf.Deg2Rad;

        boundsRadius *= displayPadding;

        float distance = boundsRadius / Mathf.Sin(fov / 2f);

        Vector3 cameraDirection = examineCamera.transform.forward;
        Vector3 newCameraPosition = boundsCenter - cameraDirection;

        examineCamera.transform.position = newCameraPosition;
        examineCamera.transform.LookAt(boundsCenter);
    }

    Bounds GetObjectBounds()
    {
        if (!examinableObject)
            Debug.LogWarning("ExamineManager.GetObjectBounds() called but there is no active examinableObject");
        
        Renderer[] renderers = examinableObject.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
            return new Bounds(examinableObject.transform.position, Vector3.zero);
        
        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            combinedBounds.Encapsulate(r.bounds);
        }

        return combinedBounds;
    }
}
