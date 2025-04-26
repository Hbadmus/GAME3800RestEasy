using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineManager : MonoBehaviour
{
    public Transform examinablePivot;
    public GameObject[] examinableObjects;
    public static ExamineManager Instance;

    Camera mainCamera;
    Camera examineCamera;
    Vector3 examineCameraOriginalPos;
    GameObject objectBeingExamined;
    Transform objectBeingExaminedOriginalTransform;
    bool examining;

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
        if (examineCamera)
            examineCameraOriginalPos = examineCamera.transform.position;

        if (!examineCamera)
            Debug.LogWarning("ExamineManager could not find the examine camera -- make sure examine camera is tagged 'ExamineCamera'");
        if (examinableObjects.Length == 0)
            Debug.LogWarning("No examinable objects added to the ExamineManager");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && examining)
        {
            Instance.ExitExamine();
        }
    }

    public void ExamineObject(int objectIdx)
    {
        examining = true;
        AudioManager.instance.ToggleSFX();

        objectBeingExamined = examinableObjects[objectIdx];
        objectBeingExaminedOriginalTransform = objectBeingExamined.transform;
        objectBeingExamined.SetActive(true);

        examineCamera.enabled = true;
        mainCamera.enabled = false;
        Time.timeScale = 0;
    }

    void ExitExamine()
    {
        examining = false;
        AudioManager.instance.ToggleSFX();

        objectBeingExamined.transform.position = objectBeingExaminedOriginalTransform.position;
        objectBeingExamined.transform.rotation = objectBeingExaminedOriginalTransform.rotation;        
        objectBeingExamined.SetActive(false);
        objectBeingExamined = null;

        examineCamera.transform.position = examineCameraOriginalPos;

        mainCamera.enabled = true;
        examineCamera.enabled = false;
        Time.timeScale = 1;
    }

    /*
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
    */
}
