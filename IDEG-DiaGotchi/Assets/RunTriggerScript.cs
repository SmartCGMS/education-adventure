using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTriggerScript : MonoBehaviour, AreaTrigger
{
    public TeacherScript TeacherObject = null;

    public void Triggered(int triggerId)
    {
        // 128, 129, 130, 131
        TeacherObject?.ArrowReached(triggerId - 128);
    }
}
