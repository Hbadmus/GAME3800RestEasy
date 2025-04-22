using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GumballMachineKey : MonoBehaviour, ICollectible
{
    public static event HandleGumballMachineKeyCollected OnGumballMachineKeyCollected;
    public delegate void HandleGumballMachineKeyCollected(ItemData itemData);
    public ItemData gumballMachineKeyData;

    public void Collect()
    {
        AudioManager.instance.PlaySFX("collecting-coin");
        //AudioSource.PlayClipAtPoint(collectCoinSFX, Camera.main.transform.position);

        Debug.Log("Gumball machine key collected");
        Destroy(gameObject);
        OnGumballMachineKeyCollected?.Invoke(gumballMachineKeyData); // in other scripts, can subscribe and unsubscribe to invoke different methods
    }
}
