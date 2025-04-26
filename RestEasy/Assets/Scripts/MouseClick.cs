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
            // Added this if (!hit) conditon because of NullReferenceExceptions (Marcella)
            if (!hit)
                return;

            Debug.Log(raycastHit.transform.name);
            if (hit && raycastHit.transform.gameObject.tag == "Collectible") 
            {
                raycastHit.transform.gameObject.GetComponentInChildren<ICollectible>().Collect();
            }
            else if (raycastHit.transform.name == "GumballPuzzle")
            {
                Debug.Log("hit gumball machine");
                AudioManager.instance.PlaySFX("coin-dispensing");
                GumballPuzzle gumballPuzzle = raycastHit.transform.gameObject.GetComponent<GumballPuzzle>();
                gumballPuzzle.CompleteGumballPuzzle();
            }
            else if (raycastHit.transform.gameObject.tag == "Book")
            {
                Debug.Log("hit book");
                AudioManager.instance.PlaySFX("book-pushed");
                BookshelfPuzzle bookshelfPuzzle = GameObject.Find("BookshelfManager").GetComponent<BookshelfPuzzle>();
                bookshelfPuzzle.IncrementBooksCollected(raycastHit.transform.gameObject);
            }
            else
            {
                //Debug.Log("Did not hit anything :(");
            }
        }
    }
}
