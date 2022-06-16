using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamedObjectScript : MonoBehaviour
{
    public int ObjectIdentifier = 0;
    public string ObjectDescription = "Object";
    public string ObjectInteractDescription = "";

    virtual public void Start()
    {
        if (ObjectIdentifier > 0)
        {
            var template = DataLoader.Current.GetObjectTemplate(ObjectIdentifier);
            if (template != null)
                ObjectDescription = template.name;
        }
    }
}
