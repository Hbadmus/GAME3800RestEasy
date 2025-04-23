using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToNotebook : MonoBehaviour
{
    public GameObject notebook;
    public string hintEnabledOnClick;
    public bool destroyColliderAfter = false;

    bool added;

    void Start()
    {
        if (!notebook) 
        {
            notebook = GameObject.FindWithTag("Notebook");
            if (!notebook)
                Debug.LogWarning("Could not find notebook");
        }
        added = false;
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnMouseDown()
    {
        if (added)
            return;
        
        NotebookController notebookController = notebook.GetComponent<NotebookController>();
        if (!notebookController)
            return;

        notebookController.EnableHint(hintEnabledOnClick);
        added = true;

        if (destroyColliderAfter)
            Destroy(GetComponent<Collider>());
    }
}
