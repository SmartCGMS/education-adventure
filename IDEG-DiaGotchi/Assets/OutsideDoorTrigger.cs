using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorTrigger : MonoBehaviour, AreaTrigger
{
    public BlackoutScript BlackoutPanel;

    public GameObject TeleportTarget = null;

    public GameObject HouseDoor = null;

    public void Triggered(int triggerId)
    {
        if (ObjectivesMgr.Current.HasActiveQuest(2))
        {
            SC_FPSController.Current.Freeze();

            BlackoutPanel.Blackout(3.0f, () => {
                if (TeleportTarget != null)
                {
                    SC_FPSController.Current.TeleportTo(TeleportTarget.transform.position, TeleportTarget.transform.rotation);
                    if (ObjectivesMgr.Current.HasActiveQuest(2))
                        PlayerStatsScript.Current.SetTime(10, 15);
                }

                if (HouseDoor != null)
                {
                    HouseDoor.GetComponent<Animator>().Play("DoorClose", 0, 0.0f);
                }
            }, () => {
                SC_FPSController.Current.Unfreeze();
            });
        }
    }
}
