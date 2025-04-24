using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BookshelfKey : MonoBehaviour, ICollectible
{
    public static event HandleBookshelfKeyCollected OnBookshelfKeyCollected;
    public delegate void HandleBookshelfKeyCollected(ItemData itemData);
    public ItemData bookshelKeyData;

    public void Collect()
    {
        AudioManager.instance.PlaySFX("collecting-key");

        Debug.Log("Bookshelf key collected");
        Destroy(gameObject);
        OnBookshelfKeyCollected?.Invoke(bookshelKeyData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
