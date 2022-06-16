using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothPasteScript : UsableObject
{
    public SinkScript SinkRef;

    public override void Start()
    {
        base.Start();

        ObjectInteractDescription = Strings.Get(4);
    }

    public override void Interact()
    {
        if (SinkRef != null)
            SinkRef.Interact();

        base.Interact();
    }
}
