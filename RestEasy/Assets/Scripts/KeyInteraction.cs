using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// New component to handle key interactions
public class KeyInteraction : MonoBehaviour
{
    private int keyIndex;
    private GameManager gameManager;
    private bool isCollected = false;

    public void Initialize(int index, GameManager manager)
    {
        keyIndex = index;
        gameManager = manager;
    }

    /*private void OnMouseDown()
    {
        if (!isCollected && gameManager != null)
        {
            isCollected = true;
            gameManager.CollectKey(keyIndex);
        }
    }*/
}
