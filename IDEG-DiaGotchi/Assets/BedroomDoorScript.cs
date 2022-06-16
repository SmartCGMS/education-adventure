using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomDoorScript : MonoBehaviour, InteractiveObject
{
    public void Interact()
    {
        SC_FPSController.Current.Talk("I should not go there, it's my parents' bedroom.");
    }
}
