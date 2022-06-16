using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : NamedObjectScript, InteractiveObject
{
    public bool RemoveAfterCollecting = false;
    public bool OneShot = true;

    private bool Collected = false;

    public override void Start()
    {
        base.Start();

        ObjectInteractDescription = Strings.Get(2);
    }

    public void Interact()
    {
        if (OneShot && Collected)
            return;

        ObjectivesMgr.Current.SignalObjective(Objectives.Collect, ObjectIdentifier, ObjectiveGroups.All);

        if (RemoveAfterCollecting)
            Destroy(transform.parent.gameObject);

        if (OneShot)
        {
            Collected = true;
            ObjectInteractDescription = "";
        }
    }
}
