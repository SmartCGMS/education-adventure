using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeteriaFoodScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public CafeteriaController.TrayState RequiredTrayState = CafeteriaController.TrayState.None;
    public int CafeteriaFoodIndex = 0;

    public void Interact()
    {
        CafeteriaController.Current.SignalFoodTaken(CafeteriaFoodIndex);
    }

    public bool PreventInteract()
    {
        return CafeteriaController.Current.trayState != RequiredTrayState;
    }
}
