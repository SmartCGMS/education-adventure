using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public void Interact()
    {
        var obj = SC_FPSController.Current.CreateHeldObject(32, gameObject, new Vector3(0.1f, 0.15f, 0.8f), Quaternion.Euler(0, 0, 0), new Vector3(0.25f, 0.25f, 0.25f));

        var comp = obj.GetComponent<TrayScript>();
        if (comp)
            comp.enabled = false;

        var comp2 = obj.GetComponent<NamedObjectScript>();
        if (comp2)
            comp2.enabled = false;

        if (CafeteriaController.Current.trayState == CafeteriaController.TrayState.Finished)
            obj.GetComponent<Animator>()?.SetInteger("TrayState", 6); // holding "full"
        else
            obj.GetComponent<Animator>()?.SetInteger("TrayState", 1); // holding "empty"

        if (CafeteriaController.Current.trayState == CafeteriaController.TrayState.Finished)
            Destroy(gameObject);
    }

    public bool PreventInteract()
    {
        return (SC_FPSController.Current.IsHoldingObject() || (CafeteriaController.Current.trayState != CafeteriaController.TrayState.None && CafeteriaController.Current.trayState != CafeteriaController.TrayState.Finished));
    }
}
