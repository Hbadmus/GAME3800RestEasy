using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour
{
    public int targetBooksToCollect =  4;
    public int booksCollected = 0;
    public GameObject secretCompartment;
    public GameObject puzzleKey;
    public GameObject ground;
    bool isPuzzleCompleted = false;

    void Update() {
        if (!isPuzzleCompleted && targetBooksToCollect == booksCollected) {
            Debug.Log("Bookshelf puzzle done!");
            isPuzzleCompleted = true;
            secretCompartment.SetActive(false);

            Rigidbody keyRB = puzzleKey.GetComponent<Rigidbody>();
            Debug.Log(puzzleKey.transform.position.y + " " + (ground.transform.position.y + 10));
            if (puzzleKey.transform.position.y < -15) {
                keyRB.isKinematic = false;
            }
            else {
                keyRB.isKinematic = true;
            }
        }
    }
    
    public void IncrementBooksCollected(GameObject hitObj) {
        Book bookScript = hitObj.GetComponent<Book>();
        if (bookScript != null && !bookScript.isClicked) {
            bookScript.isClicked = true;
            booksCollected++;
        }
    }
}
