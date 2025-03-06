using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour
{
    public int targetBooksToCollect =  5;
    public int booksCollected = 0;
    bool isPuzzleCompleted = false;

    void Update() {
        if (!isPuzzleCompleted && targetBooksToCollect == booksCollected) {
            Debug.Log("Bookshelf puzzle done!");
            isPuzzleCompleted = true;
        }
    }
    
    public void IncrementBooksCollected() {
        booksCollected++;
    }
}
