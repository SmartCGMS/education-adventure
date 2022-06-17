using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AreaTrigger
{
    public abstract void Triggered();
}

public class AreaTriggerScript : MonoBehaviour
{
    private float LastTrigger = -1.0f;
    public float TriggerTimeout = 1.0f;
    public int AreaTriggerIdentifier = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "FPSPlayer")
            return;

        if (LastTrigger > 0 && Time.time - LastTrigger < TriggerTimeout)
            return;

        LastTrigger = Time.time;

        var trig = GetComponent<AreaTrigger>();
        if (trig != null)
            trig.Triggered();

        if (AreaTriggerIdentifier > 0)
        {
            var tpl = DataLoader.Current.GetAreaTriggerTemplate(AreaTriggerIdentifier);
            if (tpl != null)
                ObjectivesMgr.Current.SignalObjective(Objectives.AreaTrigger, AreaTriggerIdentifier, ObjectiveGroups.All);
        }
    }
}
