using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeteriaDoorScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public GameObject PlaygroundTeleportTarget = null;

    public BlackoutScript BlackoutPanel = null;

    public void Interact()
    {
        SC_FPSController.Current.Freeze();

        BlackoutPanel.Blackout(3.0f, () => {
            if (PlaygroundTeleportTarget != null)
            {
                CafeteriaController.Current.ResetCafeteria(true);

                SC_FPSController.Current.TeleportTo(PlaygroundTeleportTarget.transform.position, PlaygroundTeleportTarget.transform.rotation);

                if (ObjectivesMgr.Current.HasActiveQuest(5))
                    PlayerStatsScript.Current.SetTime(14, 25);
            }
        }, () => {
            SC_FPSController.Current.Unfreeze();
        });
    }

    public bool PreventInteract()
    {
        // has "PE class" quest
        if (ObjectivesMgr.Current.HasActiveQuest(5))
            return false;

        SC_FPSController.Current.Talk(Strings.Get(124));

        return true;
    }
}
