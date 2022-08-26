using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayLineScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public void Interact()
    {
        var tray = SC_FPSController.Current.TransferHeldObject(gameObject);

        var cm = GetComponent<NamedObjectScript>();
        if (cm != null)
            cm.enabled = false;

        CafeteriaController.Current.SetTrayObject(tray);
        CafeteriaController.Current.SetTrayState(CafeteriaController.TrayState.OnRailing_1);
    }

    public bool PreventInteract()
    {
        if (SC_FPSController.Current.IsHoldingObject() && SC_FPSController.Current.HeldObjectId == 32)
            return false;

        SC_FPSController.Current.Talk(Strings.Get(102));

        return true;
    }
}
