using UnityEngine;
using System;

[Serializable]
public class BookPair
{
    public GameObject floorBook;
    public GameObject shelfBook;
}

public class BookSwitch : MonoBehaviour
{
    public BookPair[] bookPairs;  // Array of floor/shelf book pairs

    public void SwitchBook(int bookIndex)
    {
        if (bookIndex < 0 || bookIndex >= bookPairs.Length)
        {
            Debug.LogWarning("Invalid book index!");
            return;
        }

        // Get the current pair
        BookPair pair = bookPairs[bookIndex];

        // Switch the books
        pair.floorBook.SetActive(false);
        pair.shelfBook.SetActive(true);
    }

    private void OnMouseDown()
    {
        // Find which book was clicked
        for (int i = 0; i < bookPairs.Length; i++)
        {
            if (bookPairs[i].floorBook == gameObject)
            {
                SwitchBook(i);
                break;
            }
        }
    }
}