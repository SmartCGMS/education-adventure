using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorTrigger : MonoBehaviour, AreaTrigger
{
    public BlackoutScript BlackoutPanel;

    public void Triggered()
    {
        if (ObjectivesMgr.Current.HasActiveQuest(2))
        {
            // TODO: more generic approach?
            SC_FPSController.Current.Freeze();

            BlackoutPanel.Blackout(3.0f, () => {
                SC_FPSController.Current.TeleportTo(223.0f, 0.8f, -92.0f);
            }, () => {
                SC_FPSController.Current.Unfreeze();
            });
        }
    }
}
