using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public void Interact()
    {
        var obj = SC_FPSController.Current.CreateHeldObject(32, gameObject, new Vector3(0.1f, 0.15f, 0.8f), Quaternion.Euler(-90, 0, 90), new Vector3(25, 25, 25));

        var comp = obj.GetComponent<TrayScript>();
        if (comp)
            Destroy(comp);

        var comp2 = obj.GetComponent<NamedObjectScript>();
        if (comp2)
            Destroy(comp2);
    }

    public bool PreventInteract()
    {
        return (SC_FPSController.Current.IsHoldingObject() || CafeteriaController.Current.trayState != CafeteriaController.TrayState.None);
    }
}
