using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour, InteractiveObject
{
    private bool SleepInProgress = false;

    public BlackoutScript BlackoutPanel;
    public PlayerStatsScript StatsControllerScript;

    public void Interact()
    {
        if (SleepInProgress)
            return;

        SleepInProgress = true;

        BlackoutPanel.Blackout(2.0f, () => {
            StatsControllerScript.SleepFor(120);
        }, () => {
            SleepInProgress = false;
        });
    }
}
