using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorTrigger : MonoBehaviour, AreaTrigger
{
    public BlackoutScript BlackoutPanel;

    public GameObject TeleportTarget = null;

    public void Triggered()
    {
        if (ObjectivesMgr.Current.HasActiveQuest(2))
        {
            SC_FPSController.Current.Freeze();

            BlackoutPanel.Blackout(3.0f, () => {
                if (TeleportTarget != null)
                    SC_FPSController.Current.TeleportTo(TeleportTarget.transform.position, TeleportTarget.transform.rotation);
            }, () => {
                SC_FPSController.Current.Unfreeze();
            });
        }
    }
}
