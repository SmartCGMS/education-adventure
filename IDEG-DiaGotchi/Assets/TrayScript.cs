using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public void Interact()
    {
        SC_FPSController.Current.CreateHeldObject(gameObject, new Vector3(0.1f, 0.15f, 0.8f), Quaternion.Euler(-90, 0, 90), new Vector3(25, 25, 25));
    }

    public bool PreventInteract()
    {
        return (SC_FPSController.Current.IsHoldingObject());
    }
}
