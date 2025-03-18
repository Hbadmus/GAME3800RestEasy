using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseTest : MonoBehaviour
{
    public GameObject testItem;

    public void Add() {
        Instantiate(testItem, new Vector3(-7, -11, -29), Quaternion.identity);
    }

    public void Remove() {
    }
}
