using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class ExaminableObject : MonoBehaviour
{
    public GameObject examinablePrefab;

    void Start()
    {
        if (!examinablePrefab)
            this.examinablePrefab = gameObject;
        if (!examinablePrefab)
            Debug.LogWarning("Assign a prefab to ExaminableObject script on " + transform.name);
    }

    void OnMouseDown()
    {
        if (ExamineManager.Instance)
            ExamineManager.Instance.ExamineObject(examinablePrefab);
        
        else
            Debug.LogWarning("ExaminableObject script failed -- cannot find ExamineManager in the scene");
    }
}
