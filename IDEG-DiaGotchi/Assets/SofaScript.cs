using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SofaScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    public GameObject Television = null;

    private Canvas TVScreen = null;

    public void Interact()
    {
        ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -10);

        if (TVScreen != null)
        {
            TVScreen.enabled = true;
            SC_FPSController.Current.TeleportTo(transform.position, transform.rotation);
            Invoke("StopRunning", 5.0f);
        }
    }

    public bool PreventInteract()
    {
        // allow interaction when the player has "Have some fun" objective (incomplete)
        if (ObjectivesMgr.Current.HasObjectiveType(Objectives.Misc, -10, ObjectiveStates.Incomplete))
            return false;

        SC_FPSController.Current.Talk(Strings.Get(166));

        return true;
    }

    private void StopRunning()
    {
        if (TVScreen != null)
            TVScreen.enabled = false;
    }

    void Start()
    {
        if (Television != null)
            TVScreen = Television.transform.Find("Canvas")?.GetComponent<Canvas>();
        StopRunning();
    }
}
