using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseManager : MonoBehaviour
{
    public GameObject briefcase;
    public GameObject[] coins = {};
    public GameObject bookshelfKey;
    public GameObject clockKey;
    public GameObject gumballMachineKey;
    public GameObject pianoKey;

    private bool isOpen = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen) {
            briefcase.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen) {
            briefcase.SetActive(false);
            isOpen = false;
        }
    }

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
            if (inventory[i].itemData.displayName == "BookshelfKey") {
                bookshelfKey.SetActive(true);
            }
            if (inventory[i].itemData.displayName == "ClockKey") {
                clockKey.SetActive(true);
            }
            if (inventory[i].itemData.displayName == "GumballMachineKey") {
                gumballMachineKey.SetActive(true);
            }
            if (inventory[i].itemData.displayName == "PianoKey") {
                pianoKey.SetActive(true);
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
