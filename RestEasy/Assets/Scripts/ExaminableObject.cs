using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class ExaminableObject : MonoBehaviour
{
    public int examinableObjectIndex = 99;
    public bool firstClickAddsToNotebook;

    bool addedToNotebook;

    void Start()
    {
        if (examinableObjectIndex == 99)
            Debug.Log("Assign an index to all examinable objects");
    }

    void OnMouseDown()
    {
        if (firstClickAddsToNotebook)
        {
            if (!addedToNotebook)
            {
                addedToNotebook = true;
                return;
            }
        }

        if (ExamineManager.Instance && examinableObjectIndex != 99)
            ExamineManager.Instance.ExamineObject(examinableObjectIndex);
        else
            Debug.LogWarning("ExaminableObject script failed");
    }
}
