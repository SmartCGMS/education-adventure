using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour, InteractiveObject
{
    private bool SleepInProgress = false;
    private bool BlackoutIn = false;
    private float BlackoutTimer = 0.0f;

    public GameObject BlackoutPanel;

    public void Interact()
    {
        if (!SleepInProgress)
        {
            BlackoutTimer = 1.0f;
            SleepInProgress = true;
            BlackoutIn = true;
        }
    }

    public void Update()
    {
        if (SleepInProgress)
        {
            BlackoutTimer -= Time.deltaTime;

            if (BlackoutTimer < 0)
            {
                if (BlackoutIn)
                {
                    BlackoutIn = false;
                    BlackoutTimer = 1.0f;
                }
                else
                {
                    SleepInProgress = false;
                    BlackoutTimer = 0.0f;
                }
            }

            BlackoutPanel.GetComponent<CanvasGroup>().alpha = (BlackoutIn ? (1.0f - BlackoutTimer) : BlackoutTimer);
        }
    }
}
