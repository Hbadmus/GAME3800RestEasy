using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour, ICollectible
{
    public static event HandleCoinCollected OnCoinCollected;
    public delegate void HandleCoinCollected(ItemData itemData);
    public ItemData coinData;
    public AudioClip collectCoinSFX;

    public void Collect()
    {
        AudioSource.PlayClipAtPoint(collectCoinSFX, Camera.main.transform.position);
        Debug.Log("Coin collected");
        Destroy(gameObject);
        OnCoinCollected?.Invoke(coinData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
