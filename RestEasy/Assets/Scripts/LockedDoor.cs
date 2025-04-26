using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    void OnMouseDown()
    {
        AudioManager.instance.PlaySFX("door-rattle");
    }
}
