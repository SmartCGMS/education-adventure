using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorScript : MonoBehaviour, InteractiveObjectCondition
{
    public bool PreventInteract()
    {
        if (ObjectivesMgr.Current.HasActiveQuest(2))
            return false;

        SC_FPSController.Current.Talk(Strings.Get(17));

        return true;
    }
}
