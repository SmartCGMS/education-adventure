using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : NamedObjectScript, InteractiveObject
{
    public bool RemoveAfterCollecting = false;
    public bool OneShot = true;

    private bool Collected = false;

    public bool MustHaveAssociatedQuest = false;
    public ObjectiveGroups ObjectiveGroup = ObjectiveGroups.None;

    public override void Start()
    {
        base.Start();

        ObjectInteractDescription = Strings.Get(2);
    }

    public void Interact()
    {
        if (OneShot && Collected)
            return;

        if (MustHaveAssociatedQuest && !ObjectivesMgr.Current.HasObjectiveType(Objectives.Collect, ObjectIdentifier))
        {
            SC_FPSController.Current.Talk(Strings.Get(165));
            return;
        }

        ObjectivesMgr.Current.SignalObjective(Objectives.Collect, ObjectIdentifier, ObjectiveGroups.All);
        SC_FPSController.Current.RegisterCollectibleToReset(ObjectIdentifier, transform, ObjectiveGroup);

        if (RemoveAfterCollecting)
        {
            transform.GetComponent<Renderer>().enabled = false;
            transform.GetComponent<Collider>().enabled = false;
            //Destroy(transform.parent.gameObject);
        }

        if (OneShot)
        {
            Collected = true;
            ObjectInteractDescription = "";
        }
    }
}
