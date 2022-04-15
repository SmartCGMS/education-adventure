using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomDoorTrigger : MonoBehaviour, AreaTrigger
{
    public void Triggered()
    {
        if (SC_FPSController.Current.ToiletUseFlag)
        {
            SC_FPSController.Current.Talk("Ugh... I should probably wash my hands after toilet.");
            SC_FPSController.Current.ToiletUseFlag = false;
        }
    }
}
