using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockKey : MonoBehaviour, ICollectible
{
    public static event HandleClockKeyCollected OnClockKeyCollected;
    public delegate void HandleClockKeyCollected(ItemData itemData);
    public ItemData clockKeyData;

    public void Collect()
    {
        AudioManager.instance.PlaySFX("collecting-coin");
        //AudioSource.PlayClipAtPoint(collectCoinSFX, Camera.main.transform.position);

        Debug.Log("Clock key collected");
        Destroy(gameObject);
        OnClockKeyCollected?.Invoke(clockKeyData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
