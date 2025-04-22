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
        AudioManager.instance.PlaySFX("collecting-coin");
        //AudioSource.PlayClipAtPoint(collectCoinSFX, Camera.main.transform.position);

        Debug.Log("Bookshelf key collected");
        Destroy(gameObject);
        OnBookshelfKeyCollected?.Invoke(bookshelKeyData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
