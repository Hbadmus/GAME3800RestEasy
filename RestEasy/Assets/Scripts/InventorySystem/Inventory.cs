using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    public static event Action<List<InventoryItem>> OnInventoryChange;

    public List<InventoryItem> inventory; // TODO: make sure can only fit set number?
    public Dictionary<ItemData, InventoryItem> itemDictionary;
    public GameObject briefcase;

    void Start()
    {
        inventory = new List<InventoryItem>();
        itemDictionary = new Dictionary<ItemData, InventoryItem>();
    }

    private void OnEnable()
    {
        BookshelfKey.OnBookshelfKeyCollected += Add;
        Coin.OnCoinCollected += Add;
        ClockKey.OnClockKeyCollected += Add;
        GumballMachineKey.OnGumballMachineKeyCollected += Add;
        PianoKey.OnPianoKeyCollected += Add;
        Ballerina.OnBallerinaCollected += Add;
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected -= Add;
        BookshelfKey.OnBookshelfKeyCollected -= Add;
        ClockKey.OnClockKeyCollected -= Add;
        GumballMachineKey.OnGumballMachineKeyCollected -= Add;
        PianoKey.OnPianoKeyCollected -= Add;
        Ballerina.OnBallerinaCollected -= Add;
    }

    public void Add(ItemData itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.AddToStack();
            Debug.Log($"{item.itemData.displayName} total stack is now {item.stackSize}");
            OnInventoryChange?.Invoke(inventory);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            itemDictionary.Add(itemData, newItem);
            Debug.Log($"Added {itemData.displayName} to the inventory for the first time.");
            OnInventoryChange?.Invoke(inventory);
        }
    }

    public void Remove(ItemData itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveFromStack();

            if (item.stackSize == 0)
            {
                inventory.Remove(item);
                itemDictionary.Remove(itemData);
            }

            OnInventoryChange?.Invoke(inventory);
        }
    }
}
