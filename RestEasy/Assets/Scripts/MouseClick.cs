using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;

            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            RaycastHit raycastHit;

            bool hit = Physics.Raycast(ray, out raycastHit);

            if (hit) //TODO: make only for ICollectible so no errors pop up
            {
                Debug.Log(raycastHit.transform.name);
                raycastHit.transform.gameObject.GetComponentInChildren<ICollectible>().Collect();
            }
            else
            {
                Debug.Log("Did not hit anything :(");
            }
        }
    }
}
