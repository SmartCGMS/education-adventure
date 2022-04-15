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

    private void OnTriggerEnter(Collider other)
    {
        if (LastTrigger > 0 && Time.time - LastTrigger < TriggerTimeout)
            return;

        var trig = GetComponent<AreaTrigger>();
        if (trig != null)
            trig.Triggered();
    }
}
