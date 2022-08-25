using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassroomDoorScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public GameObject CafeteriaTeleportTarget = null;

    public BlackoutScript BlackoutPanel = null;

    public void Interact()
    {
        SC_FPSController.Current.Freeze();

        BlackoutPanel.Blackout(3.0f, () => {
            if (CafeteriaTeleportTarget != null)
            {
                SC_FPSController.Current.TeleportTo(CafeteriaTeleportTarget.transform.position, CafeteriaTeleportTarget.transform.rotation);

                if (ObjectivesMgr.Current.HasActiveQuest(4))
                    PlayerStatsScript.Current.SetTime(12, 25);
            }
        }, () => {
            SC_FPSController.Current.Unfreeze();
        });
    }

    public bool PreventInteract()
    {
        // has "Lunch time" quest
        if (ObjectivesMgr.Current.HasActiveQuest(4))
            return false;

        SC_FPSController.Current.Talk(Strings.Get(97));

        return true;
    }
}
