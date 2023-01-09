using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaptopScript : MonoBehaviour, InteractiveObject, InteractiveObjectCondition
{
    private Canvas LaptopScreen = null;

    public void Interact()
    {
        ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -10);

        if (LaptopScreen != null)
        {
            LaptopScreen.enabled = true;
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
        if (LaptopScreen != null)
            LaptopScreen.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        LaptopScreen = transform.Find("Canvas")?.GetComponent<Canvas>();
        StopRunning();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
