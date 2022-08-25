using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherScript : MonoBehaviour, InteractiveObject
{
    private bool LookingAtPlayer = false;

    public void Interact()
    {
        if (LookingAtPlayer)
            return;

        if (ObjectivesMgr.Current.HasActiveQuest(3))
        {
            LookingAtPlayer = true;
            Invoke("StopLooking", 3.0f);

            SC_FPSController.Current.TalkAll(5);
        }
    }

    public void StopLooking()
    {
        LookingAtPlayer = false;
    }

    public void LateUpdate()
    {
        if (LookingAtPlayer)
        {
            var obj = gameObject.transform.Find("Armature/SpineBottom/SpineTop/Head");
            if (obj != null)
            {
                obj.transform.LookAt(SC_FPSController.Current.gameObject.transform);
            }
        }
    }
}
