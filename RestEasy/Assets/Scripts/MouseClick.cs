using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    public Camera mainCamera;
    public AudioClip coinInDispenserSFX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
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

            //Debug.Log(raycastHit.transform.name);
            if (hit && raycastHit.transform.gameObject.tag == "Collectible") 
            {
                raycastHit.transform.gameObject.GetComponentInChildren<ICollectible>().Collect();
            }
            else if (hit && raycastHit.transform.gameObject.tag == "GumballMachine")
            {
                AudioSource.PlayClipAtPoint(coinInDispenserSFX, Camera.main.transform.position);
                GumballPuzzle gumballPuzzle = raycastHit.transform.gameObject.GetComponent<GumballPuzzle>();
                gumballPuzzle.CompleteGumballPuzzle();
            }
            else if (hit && raycastHit.transform.gameObject.tag == "Book")
            {
                Debug.Log("hit book");
                BookshelfPuzzle bookshelfPuzzle = GameObject.Find("BookshelfManager").GetComponent<BookshelfPuzzle>();
                bookshelfPuzzle.IncrementBooksCollected();
            }
            else
            {
                //Debug.Log("Did not hit anything :(");
            }
        }
    }
}
