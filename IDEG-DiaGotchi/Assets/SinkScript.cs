using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkScript : MonoBehaviour, InteractiveObject
{
    public ParticleSystem StreamParticles;

    public void Interact()
    {
        SC_FPSController.Current.ToiletUseFlag = false;

        if (StreamParticles != null)
            StreamParticles.Play();
    }
}
