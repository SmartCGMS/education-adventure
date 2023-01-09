using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabetologistDoorScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public GameObject HomeTeleportTarget = null;

    public BlackoutScript BlackoutPanel = null;

    public void Interact()
    {
        SC_FPSController.Current.Freeze();

        BlackoutPanel.Blackout(3.0f, () => {
            if (HomeTeleportTarget != null)
            {
                CafeteriaController.Current.ResetCafeteria(true);

                SC_FPSController.Current.ResetCollectibles(ObjectiveGroups.Home);
                SC_FPSController.Current.TeleportTo(HomeTeleportTarget.transform.position, HomeTeleportTarget.transform.rotation);

                PlayerStatsScript.Current.SetTime(19, 15);
            }
        }, () => {
            SC_FPSController.Current.Unfreeze();
        });
    }

    public bool PreventInteract()
    {
        // has "diabetologist" quest
        if (ObjectivesMgr.Current.HasActiveQuest(6))
        {
            SC_FPSController.Current.Talk(Strings.Get(161));
            return true;
        }

        return false;
    }
}
