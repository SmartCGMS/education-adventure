using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableObject : NamedObjectScript, InteractiveObject
{
    public int InteractStringId = 6;

    public override void Start()
    {
        base.Start();

        ObjectInteractDescription  = Strings.Get(InteractStringId);
    }

    public virtual void Interact()
    {
        ObjectivesMgr.Current.SignalObjective(Objectives.Use, ObjectIdentifier, ObjectiveGroups.All);
    }
}
