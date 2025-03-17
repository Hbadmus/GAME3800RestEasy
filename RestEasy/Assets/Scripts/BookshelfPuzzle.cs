using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour
{
    public int targetBooksToCollect =  5;
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

        }
        else if (isPuzzleCompleted) {
            Rigidbody keyRB = puzzleKey.GetComponent<Rigidbody>();
            if (puzzleKey.transform.position.y > ground.transform.position.y) {
                keyRB.isKinematic = false;
            }
            else {
                keyRB.isKinematic = true;
            }
        }
    }
    
    public void IncrementBooksCollected() {
        booksCollected++;
    }
}
