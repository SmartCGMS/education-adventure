using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeteriaDeskScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public void Interact()
    {
        if (CafeteriaController.Current.trayState == CafeteriaController.TrayState.Finished)
        {
            SC_FPSController.Current.ClearHeldObjects();

            CafeteriaController.Current.SetTrayState(CafeteriaController.TrayState.None);
        }
    }

    public bool PreventInteract()
    {
        return (CafeteriaController.Current.trayState != CafeteriaController.TrayState.Finished);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
