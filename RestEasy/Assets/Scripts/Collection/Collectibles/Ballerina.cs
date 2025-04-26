using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ballerina : MonoBehaviour, ICollectible
{
    public static event HandleBallerinaCollected OnBallerinaCollected;
    public delegate void HandleBallerinaCollected(ItemData itemData);
    public ItemData ballerinaData;

    public void Collect()
    {
        AudioManager.instance.PlaySFX("collecting-coin");

        Debug.Log("Ballerina collected");
        OnBallerinaCollected?.Invoke(ballerinaData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
