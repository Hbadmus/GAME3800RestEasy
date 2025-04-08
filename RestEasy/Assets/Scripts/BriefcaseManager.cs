using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseManager : MonoBehaviour
{
    public GameObject[] coins = {};

    private void OnEnable()
    {
        Inventory.OnInventoryChange += UpdateBriefcase;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChange -= UpdateBriefcase;
    }

    void UpdateBriefcase(List<InventoryItem> inventory)
    {
        Debug.Log("here");
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].itemData.displayName == "Coin") {
                Debug.Log(inventory[i].stackSize);
                UpdateCoins(inventory[i].stackSize);
            }
        }
    }

    void UpdateCoins(int size) {
        for (int i = 0; i < size; i++) {
            Debug.Log("HERE " + coins[i].name);
            coins[i].SetActive(true);
        }

        for (int j = size; j < coins.Length; j++) {
            coins[j].SetActive(false);
        }
    }
}
