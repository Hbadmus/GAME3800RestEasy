using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSection : MonoBehaviour
{
    public GameObject clock;
    public int number;

    void Start()
    {
        if (!clock)
            Debug.Log("ClockSection script is missing clock game object value");
        if (number == 0)
            Debug.Log("ClockSection script is missing number value");
    }

    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("Clock section " + number + " clicked");
        ClockPuzzle clockPuzzle = clock.GetComponent<ClockPuzzle>();
        if (clockPuzzle)
            clockPuzzle.SectionClicked(number);
    }
}
 