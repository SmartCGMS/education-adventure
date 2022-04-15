using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour, InteractiveObject
{
    public Light SwitchLight;
    public GameObject LightObject;

    public void Interact()
    {
        transform.parent.Rotate(0, 0, 180);

        if (SwitchLight != null && LightObject != null)
        {
            var l = SwitchLight.GetComponent<Light>();
            l.enabled = !l.enabled;

            if (!l.enabled)
                LightObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            else
                LightObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        }
    }
}
