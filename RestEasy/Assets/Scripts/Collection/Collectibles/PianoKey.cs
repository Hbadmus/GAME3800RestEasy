using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PianoKey : MonoBehaviour, ICollectible
{
    public static event HandlePianoCollected OnPianoKeyCollected;
    public delegate void HandlePianoCollected(ItemData itemData);
    public ItemData pianoKeyData;

    public void Collect()
    {
        AudioManager.instance.PlaySFX("collecting-key");
        //AudioSource.PlayClipAtPoint(collectCoinSFX, Camera.main.transform.position);

        Debug.Log("Piano key collected");
        Destroy(gameObject);
        OnPianoKeyCollected?.Invoke(pianoKeyData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
