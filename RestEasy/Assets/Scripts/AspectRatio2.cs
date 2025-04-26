using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AspectRatio2 : MonoBehaviour
{
    public GameObject parent;
    public bool changeScale = false;
    public float xRatio = 1;
    public float yRatio = 1;
    public float xPosRatio = 1;
    public float yPosRatio = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (changeScale) ChangeScale();
        else ChangeSize();
    }

    public void ChangeScale() {
        RectTransform parentTransform = (RectTransform)parent.transform;
        RectTransform thisTransform = (RectTransform)transform;

        float scaleY = parentTransform.sizeDelta.y / thisTransform.sizeDelta.y;

        thisTransform.localScale = new Vector3(thisTransform.localScale.x * scaleY, thisTransform.localScale.y * scaleY, 1);
    }

    public void ChangeSize() {
        RectTransform parentTransform = (RectTransform)parent.transform;

        Debug.Log(parentTransform.rect.width + " " + parentTransform.rect.height);

        RectTransform thisTransform = (RectTransform)transform;
        thisTransform.sizeDelta = new Vector2(parentTransform.rect.width * xRatio, parentTransform.rect.height * yRatio);

        thisTransform.position = new Vector2(parentTransform.position.x * xPosRatio, parentTransform.position.y * yPosRatio);
    }
}

    // For custom editor (Volt Editor Attribute)
    [CustomEditor(typeof(AspectRatio2))]
    public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Change Size:", EditorStyles.boldLabel);
            if (GUILayout.Button("Change Size"))
            {
                ((AspectRatio2)target).ChangeSize();
            }
            EditorGUILayout.EndVertical();
        }
    }
