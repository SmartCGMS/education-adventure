using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglerScript : MonoBehaviour
{
    public string OpenAnimationName = "DoorOpen";
    public string CloseAnimationName = "DoorClose";

    private Animator animator;

    private bool isOpen = false;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void ToggleState()
    {
        if (isOpen)
        {
            animator.Play(CloseAnimationName, 0, 0.0f);
            isOpen = false;
        }
        else
        {
            animator.Play(OpenAnimationName, 0, 0.0f);
            isOpen = true;
        }
    }
}
