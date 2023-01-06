using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundDoorScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public GameObject DiabetologistTeleportTarget = null;
    public GameObject HomeTeleportTarget = null;

    public BlackoutScript BlackoutPanel = null;

    public void Interact()
    {
        SC_FPSController.Current.Freeze();

        BlackoutPanel.Blackout(3.0f, () => {
            // 6 = diabetologist quest
            if (ObjectivesMgr.Current.HasActiveQuest(6))
            {
                if (DiabetologistTeleportTarget != null)
                {
                    CafeteriaController.Current.ResetCafeteria(true);
                    SC_FPSController.Current.TeleportTo(DiabetologistTeleportTarget.transform.position, DiabetologistTeleportTarget.transform.rotation);
                    PlayerStatsScript.Current.SetTime(16, 30);
                }
            }
            else
            {
                if (HomeTeleportTarget != null)
                {
                    CafeteriaController.Current.ResetCafeteria(true);
                    SC_FPSController.Current.TeleportTo(HomeTeleportTarget.transform.position, HomeTeleportTarget.transform.rotation);
                    PlayerStatsScript.Current.SetTime(19, 10);
                }
            }
        }, () => {
            SC_FPSController.Current.Unfreeze();
        });
    }

    public bool PreventInteract()
    {
        // has "PE class" quest - during it, you cannot leave
        if (ObjectivesMgr.Current.HasActiveQuest(5))
        {
            SC_FPSController.Current.Talk(Strings.Get(160));
            return true;
        }

        return false;
    }
}
