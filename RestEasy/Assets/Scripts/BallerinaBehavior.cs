using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallerinaBehavior : MonoBehaviour
{
    Animator animator;
    public bool isPlaying;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        if (isPlaying)
        {
            animator.SetTrigger("closed");
            isPlaying = false;
        }
        else
        {
            animator.SetTrigger("opened");
            isPlaying = true;
        }
    }
}
