using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherScript : MonoBehaviour, InteractiveObject
{
    private bool LookingAtPlayer = false;

    private bool PEInProgress = false;
    private int activeArrow = -1;
    private int remainingLaps = 3;

    public List<GameObject> arrowObjects = new List<GameObject>();

    public void Start()
    {
        if (arrowObjects != null && arrowObjects.Count > 0)
        {
            for (int i = 0; i < arrowObjects.Count; i++)
                SetArrowState(i, false);
        }
    }

    void SetArrowState(int index, bool enabled)
    {
        foreach (Transform child in arrowObjects[index].transform)
        {
            var renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.enabled = enabled;
        }

        if (enabled)
            activeArrow = index;
    }

    public void ArrowReached(int index)
    {
        if (!PEInProgress)
            return;

        if (index < 0 || index >= arrowObjects.Count || index != activeArrow)
            return;

        SetArrowState(index, false);

        if (index == 3)
        {
            remainingLaps--;
            if (remainingLaps <= 0)
            {
                SC_FPSController.Current.PerformScriptedAction(4);
                GetComponent<Animator>().SetBool("IsCheckingClass", false);
                GetComponent<Animator>().SetBool("IsClassInProgress", false);
                return;
            }
        }

        SetArrowState((index + 1) % arrowObjects.Count, true);
    }

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

        if (ObjectivesMgr.Current.HasActiveQuest(5))
        {
            LookingAtPlayer = true;
            Invoke("StopLooking", 3.0f);

            SC_FPSController.Current.TalkAll(7);

            if (!PEInProgress)
            {
                PEInProgress = true;
                remainingLaps = 3;
                GetComponent<Animator>().SetBool("IsCheckingClass", true);
                GetComponent<Animator>().SetBool("IsClassInProgress", true);

                SetArrowState(0, true);
            }
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
