using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GumballPuzzle : MonoBehaviour
{
    public Inventory inventory;
    public ItemData coinData;
    public int coinCount = 4;

    public AudioClip coinDispensingSFX;
    public AudioClip successSFX;

    private static int coinsCollectedSoFar = 0;
    private bool finishedPuzzle = false;

    public void CompleteGumballPuzzle()
    {
        if (inventory.itemDictionary.TryGetValue(coinData, out InventoryItem item) && !finishedPuzzle)
        {
            int originalStackSize = item.stackSize;
            for (int i = 0; i < originalStackSize; i++)
            {
                coinsCollectedSoFar++;
                inventory.Remove(coinData);
            }

            if (coinsCollectedSoFar == coinCount)
            {
                finishedPuzzle = true;
                Debug.Log("Congrats!");
                AudioSource.PlayClipAtPoint(coinDispensingSFX, Camera.main.transform.position);
                AudioSource.PlayClipAtPoint(successSFX, Camera.main.transform.position);
            }
        }
    }
}
