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
        AudioManager.instance.PlaySFX("collecting-key");

        Debug.Log("Clock key collected");
        Destroy(gameObject);
        OnClockKeyCollected?.Invoke(clockKeyData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
