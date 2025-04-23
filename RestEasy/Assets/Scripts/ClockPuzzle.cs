using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockPuzzle : MonoBehaviour
{
    public GameObject clockHand;
    public GameObject door;
    public List<int> correctOrder = new List<int> { 3, 9, 3, 6 };

    List<int> lastSectionsClicked = new List<int>();
    List<float> numberDegrees = new List<float> { -60f, -30f, 0f, 30f, 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f };
    float timeSinceLastClick = 0f;
    bool puzzleSolved = false;
    Animator animator;
    public GameObject puzzleKey;


    void Start()
    {
        if (!clockHand || !door)
            Debug.Log("Assign all ClockPuzzle variables values in the inspector");

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Debug.Log("Time since last click: " + timeSinceLastClick);

        bool correctNumbersClicked = CorrectNumbersClicked();
        if (correctNumbersClicked)
            PuzzleSolved();
        
        if (timeSinceLastClick > 0f)
            timeSinceLastClick -= Time.deltaTime;
        
        if (timeSinceLastClick <= 0f)
        {
            timeSinceLastClick = 0f;
            lastSectionsClicked = new List<int>();
        }
    }

    public void SectionClicked(int section)
    {
        timeSinceLastClick = 5f;

        RotateToNumber(section);
    }

    void RotateToNumber(int number)
    {
        lastSectionsClicked.Add(number);

        float targetRotationAngle = numberDegrees[number - 1];
        Quaternion currentRotation = clockHand.transform.rotation;

        Quaternion targetRotation = Quaternion.Euler(targetRotationAngle, currentRotation.y, currentRotation.z);
        
        clockHand.transform.rotation = targetRotation;
    }

    bool CorrectNumbersClicked()
    {
        if (puzzleSolved) // to avoid door animation happening multiple times
            return false;

        if (lastSectionsClicked.Count < 4)
            return false;
        List<int> lastFourSectionsClicked = lastSectionsClicked.GetRange(lastSectionsClicked.Count - 4, 4);

        for (int i = 0; i < 4; i++)
        {
            if (lastFourSectionsClicked[i] != correctOrder[i])
                return false;
        }
        
        return true;
    }

    void PuzzleSolved()
    {
        animator.SetTrigger("puzzleSolved");
        AudioManager.instance.PlaySFX("puzzle-success");
        puzzleSolved = true;
        puzzleKey.SetActive(true);
    }

    void OnAnimationEnd()
    {
        animator.speed = 0;
    }
}
