using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentScript : MonoBehaviour, InteractiveObject
{
    private bool LookingAtPlayer = false;

    public void Interact()
    {
        LookingAtPlayer = true;
        Invoke("StopLooking", 2.0f);

        SC_FPSController.Current.Talk(Strings.Get(Random.Range(94, 96 +1/*exclusive*/)), DataLoader.TalkAction.None, 0, 2.0f);
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
