using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeScript : MonoBehaviour, InteractiveObject
{
    public GameObject FoodUI;

    public void Interact()
    {
        var anim = FoodUI.GetComponent<Animator>();
        if (anim != null)
        {
            SC_FPSController.Current.Freeze();
            anim.Play("UIPanelAppear");
        }
    }
}
