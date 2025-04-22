using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool startOpen = false;
    Animator animator;
    bool open = false;

    void Start()
    {
        if (startOpen)
        {
            open = true;
        }
        animator = GetComponentInParent<Animator>();
        if (!animator)
            Debug.LogWarning("Attach an animator component to door's parent object");

        if (startOpen)
        {
            animator.SetTrigger("startOpen");
            open = true;
        }
    }

    public void SlamDoor()
    {
        if (AnimPlaying())
            return;
        
        if (!open)
            return;

        animator.SetTrigger("slamDoor");
        AudioManager.instance.PlaySFX("door-slam", 0);
        open = false;
    }

    void OnMouseDown()
    {
        if (AnimPlaying())
            return;
        
        if (open)
        {
            animator.SetTrigger("closeDoor");
            AudioManager.instance.PlaySFX("door-close", 0);
            open = false;
        }
        else
        {
            animator.SetTrigger("openDoor");
            AudioManager.instance.PlaySFX("door-open", 0);
            open = true;
        }
    }

    bool AnimPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }
}
