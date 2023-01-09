using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeteriaDeskScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public BlackoutScript BlackoutPanel = null;

    public void Interact()
    {
        if (CafeteriaController.Current.trayState == CafeteriaController.TrayState.Finished)
        {
            BlackoutPanel.Blackout(3.0f, () => {
                SC_FPSController.Current.Freeze();

                SC_FPSController.Current.ClearHeldObjects();

                CafeteriaController.Current.SetTrayState(CafeteriaController.TrayState.None);
            }, () => {
                SC_FPSController.Current.Unfreeze();
            });
        }
    }

    public bool PreventInteract()
    {
        if (CafeteriaController.Current.trayState != CafeteriaController.TrayState.Finished)
        {
            SC_FPSController.Current.Talk(Strings.Get(167));
            return true;
        }

        return false;
    }
}
